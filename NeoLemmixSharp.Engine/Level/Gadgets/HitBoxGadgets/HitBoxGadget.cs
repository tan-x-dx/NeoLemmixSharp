using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Gadgets.Interfaces;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

#pragma warning disable CS0660, CS0661, CA1067
public sealed class HitBoxGadget : GadgetBase,
    IIdEquatable<HitBoxGadget>,
    IPreviousRectangularBounds,
    IMoveableGadget
#pragma warning restore CS0660, CS0661, CA1067
{
    private readonly LemmingTracker _lemmingTracker;
    private readonly GadgetState[] _states;

    private GadgetState _currentState;
    private GadgetState _previousState;

    private int _currentStateIndex;
    private int _nextStateIndex;

    public GadgetState CurrentState => _currentState;

    // The below properties refer to the positions of the hitboxes, not the gadget itself
    public LevelRegion CurrentBounds => _currentState.GetMininmumBoundingBoxForAllHitBoxes(CurrentGadgetBounds.Position);
    public LevelRegion PreviousBounds => _previousState.GetMininmumBoundingBoxForAllHitBoxes(PreviousGadgetBounds.Position);

    public ResizeType ResizeType { get; }

    public HitBoxGadget(
        ResizeType resizeType,
        LemmingTracker lemmingTracker,
        GadgetState[] states,
        int initialStateIndex)
        : base(0)
    {
        _lemmingTracker = lemmingTracker;
        _states = states;

        _currentStateIndex = initialStateIndex;
        _currentState = _states[initialStateIndex];
        CurrentAnimationController = _currentState.AnimationController;
        _previousState = _currentState;

        ResizeType = resizeType;
    }

    public void SetNextState(int stateIndex)
    {
        _nextStateIndex = stateIndex;
    }

    public override void Tick()
    {
        _lemmingTracker.Tick();

        if (_currentStateIndex == _nextStateIndex)
        {
            CurrentState.Tick(this);
        }
        else
        {
            ChangeStates();
        }
    }

    private void ChangeStates()
    {
        _currentStateIndex = _nextStateIndex;

        PreviousGadgetBounds.SetFrom(CurrentGadgetBounds);
        _previousState = _currentState;

        _currentState = _states[_currentStateIndex];

        _previousState.OnTransitionFrom();
        _currentState.OnTransitionTo();
        CurrentAnimationController = _currentState.AnimationController;

        // Changing states may change hitbox positions 
        // Force a position update to accommodate this
        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public bool ContainsPoint(Orientation orientation, LevelPosition levelPosition)
    {
        return _currentState
            .HitBoxFor(orientation)
            .ContainsPoint(levelPosition - CurrentGadgetBounds.Position);
    }

    public bool ContainsPoints(Orientation orientation, LevelPosition p1, LevelPosition p2)
    {
        var hitBox = _currentState.HitBoxFor(orientation);
        return hitBox.ContainsPoint(p1 - CurrentGadgetBounds.Position) ||
               hitBox.ContainsPoint(p2 - CurrentGadgetBounds.Position);
    }

    public void OnLemmingHit(
        LemmingHitBoxFilter activeFilter,
        Lemming lemming)
    {
        var actionsToPerform = GetActionsToPerformOnLemming(activeFilter, lemming);

        for (var i = 0; i < actionsToPerform.Length; i++)
        {
            actionsToPerform[i].PerformAction(lemming);
        }
    }

    private ReadOnlySpan<IGadgetAction> GetActionsToPerformOnLemming(
        LemmingHitBoxFilter activeFilter,
        Lemming lemming)
    {
        var trackingStatus = _lemmingTracker.TrackItem(lemming);

        return trackingStatus switch
        {
            TrackingStatus.Absent => ReadOnlySpan<IGadgetAction>.Empty,
            TrackingStatus.JustAdded => activeFilter.OnLemmingEnterActions,
            TrackingStatus.JustRemoved => activeFilter.OnLemmingExitActions,
            TrackingStatus.StillPresent => activeFilter.OnLemmingPresentActions,

            _ => ReadOnlySpan<IGadgetAction>.Empty
        };
    }

    public void Move(int dx, int dy)
    {
        PreviousGadgetBounds.SetFrom(CurrentGadgetBounds);
        _previousState = _currentState;

        CurrentGadgetBounds.X = LevelScreen.HorizontalBoundaryBehaviour.Normalise(CurrentGadgetBounds.X + dx);
        CurrentGadgetBounds.Y = LevelScreen.VerticalBoundaryBehaviour.Normalise(CurrentGadgetBounds.Y + dy);
        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public void SetPosition(int x, int y)
    {
        PreviousGadgetBounds.SetFrom(CurrentGadgetBounds);
        _previousState = _currentState;

        CurrentGadgetBounds.X = LevelScreen.HorizontalBoundaryBehaviour.Normalise(x);
        CurrentGadgetBounds.Y = LevelScreen.VerticalBoundaryBehaviour.Normalise(y);
        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public void Resize(int dw, int dh)
    {
        if (ResizeType == ResizeType.None)
            return;

        PreviousGadgetBounds.SetFrom(CurrentGadgetBounds);
        _previousState = _currentState;

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

        PreviousGadgetBounds.SetFrom(CurrentGadgetBounds);
        _previousState = _currentState;

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
