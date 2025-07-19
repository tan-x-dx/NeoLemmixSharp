using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.LemmingBehaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Gadgets.Interfaces;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

#pragma warning disable CS0660, CS0661, CA1067
public sealed class HitBoxGadget : GadgetBase,
    IIdEquatable<HitBoxGadget>,
    IRectangularBounds,
    IMoveableGadget
#pragma warning restore CS0660, CS0661, CA1067
{
    private readonly LemmingTracker _lemmingTracker;
    private readonly HitBoxGadgetState[] _states;

    private HitBoxGadgetState _currentState;
    private HitBoxGadgetState _previousState;

    private int _currentStateIndex;
    private int _nextStateIndex;

    // The below properties refer to the positions of the hitboxes, not the gadget itself
    public RectangularRegion CurrentBounds => CurrentState.GetMininmumBoundingBoxForAllHitBoxes(CurrentGadgetBounds.Position);

    public ResizeType ResizeType { get; }

    public HitBoxGadget(
        string gadgetName,
        HitBoxGadgetState[] states,
        int initialStateIndex,
        ResizeType resizeType,
        LemmingTracker lemmingTracker)
        : base(gadgetName)
    {
        _lemmingTracker = lemmingTracker;
        _states = states;

        _currentState = states[initialStateIndex];
        _previousState = _currentState;
        _currentStateIndex = initialStateIndex;
        _nextStateIndex = initialStateIndex;

        ResizeType = resizeType;

        foreach (var state in states)
        {
            state.SetParentGadget(this);
        }
    }

    public override HitBoxGadgetState CurrentState => _currentState;

    public override void Tick()
    {
        _lemmingTracker.Tick();

        if (_currentStateIndex != _nextStateIndex)
            ChangeStates();

        CurrentState.Tick();
    }

    private void ChangeStates()
    {
        _currentStateIndex = _nextStateIndex;

        _previousState = _currentState;
        _currentState = _states[_currentStateIndex];

        _previousState.OnTransitionFrom();
        _currentState.OnTransitionTo();
        _previousState = _currentState;

        // Changing states may change hitbox positions 
        // Force a position update to accommodate this
        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    private void UpdatePreviousState()
    {
        _previousState = _currentState;
    }

    public override void SetNextState(int stateIndex)
    {
        _nextStateIndex = stateIndex;
    }

    public bool ContainsPoint(Orientation orientation, Point levelPosition)
    {
        return CurrentState
            .HitBoxFor(orientation)
            .ContainsPoint(levelPosition - CurrentGadgetBounds.Position);
    }

    public bool ContainsEitherPoint(Orientation orientation, Point p1, Point p2)
    {
        var offset = CurrentGadgetBounds.Position;
        var hitBox = CurrentState.HitBoxFor(orientation);
        return hitBox.ContainsEitherPoint(p1 - offset, p2 - offset);
    }

    public void OnLemmingHit(
        LemmingHitBoxFilter activeFilter,
        Lemming lemming)
    {
        var actionsToPerform = GetActionsToPerformOnLemming(activeFilter, lemming);

        for (var i = 0; i < actionsToPerform.Length; i++)
        {
            actionsToPerform[i].PerformBehaviour(lemming);
        }

        var generalBehaviours = activeFilter.Behaviours;

        foreach (var behaviour in generalBehaviours)
        {
            behaviour.PerformBehaviour();
        }
    }

    private ReadOnlySpan<LemmingBehaviour> GetActionsToPerformOnLemming(
        LemmingHitBoxFilter activeFilter,
        Lemming lemming)
    {
        var trackingStatus = _lemmingTracker.TrackItem(lemming);

        return trackingStatus switch
        {
            TrackingStatus.Absent => ReadOnlySpan<LemmingBehaviour>.Empty,
            TrackingStatus.Entered => activeFilter.OnLemmingEnterActions,
            TrackingStatus.Exited => activeFilter.OnLemmingExitActions,
            TrackingStatus.StillPresent => activeFilter.OnLemmingPresentActions,

            _ => ReadOnlySpan<LemmingBehaviour>.Empty
        };
    }

    public void Move(int dx, int dy)
    {
        UpdatePreviousState();

        var delta = new Point(dx, dy);
        CurrentGadgetBounds.Position = LevelScreen.NormalisePosition(CurrentGadgetBounds.Position + delta);
        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public void SetPosition(int x, int y)
    {
        UpdatePreviousState();

        var newPosition = new Point(x, y);
        CurrentGadgetBounds.Position = LevelScreen.NormalisePosition(newPosition);
        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public void Resize(int dw, int dh)
    {
        if (ResizeType == ResizeType.None)
            return;

        UpdatePreviousState();

        if (ResizeType.CanResizeHorizontally())
            CurrentGadgetBounds.Width += dw;

        if (ResizeType.CanResizeVertically())
            CurrentGadgetBounds.Height += dh;

        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public void SetSize(int w, int h)
    {
        if (ResizeType == ResizeType.None)
            return;

        UpdatePreviousState();

        if (ResizeType.CanResizeHorizontally())
            CurrentGadgetBounds.Width = w;

        if (ResizeType.CanResizeVertically())
            CurrentGadgetBounds.Height = h;

        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public bool Equals(HitBoxGadget? other) => Id == (other?.Id ?? -1);

    public static bool operator ==(HitBoxGadget left, HitBoxGadget right) => left.Id == right.Id;
    public static bool operator !=(HitBoxGadget left, HitBoxGadget right) => left.Id != right.Id;
}
