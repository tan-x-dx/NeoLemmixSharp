using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class HitBoxGadget : GadgetBase, IRectangularBounds, IMoveableGadget
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
        HitBoxGadgetState[] states,
        int initialStateIndex,
        ResizeType resizeType,
        LemmingTracker lemmingTracker)
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
        var causeAndEffectManager = LevelScreen.CauseAndEffectManager;

        var lemmingBehaviourIds = activeFilter.OnLemmingHitBehaviourIds;
        foreach (var behaviourId in lemmingBehaviourIds)
        {
            causeAndEffectManager.RegisterCauseAndEffectData(new CauseAndEffectData(behaviourId, lemming.Id));
        }

        var hitBoxModifierActionsToPerform = GetHitBoxModifierActionsToPerformOnLemming(activeFilter, lemming);
        foreach (var behaviourId in hitBoxModifierActionsToPerform)
        {
            causeAndEffectManager.RegisterCauseAndEffectData(new CauseAndEffectData(behaviourId, lemming.Id));
        }
    }

    private ReadOnlySpan<int> GetHitBoxModifierActionsToPerformOnLemming(
        LemmingHitBoxFilter activeFilter,
        Lemming lemming)
    {
        var trackingStatus = _lemmingTracker.TrackItem(lemming);

        return trackingStatus switch
        {
            TrackingStatus.Absent => ReadOnlySpan<int>.Empty,
            TrackingStatus.Entered => activeFilter.OnLemmingEnterBehaviourIds,
            TrackingStatus.Exited => activeFilter.OnLemmingExitBehaviourIds,
            TrackingStatus.StillPresent => activeFilter.OnLemmingPresentBehaviourIds,

            _ => ReadOnlySpan<int>.Empty
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
