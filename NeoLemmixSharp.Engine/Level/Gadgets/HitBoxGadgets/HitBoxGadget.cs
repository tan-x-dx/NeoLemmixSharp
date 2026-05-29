using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.Movement;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
public sealed class HitBoxGadget : GadgetBase, IRectangularBounds, IMoveableGadget
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
{
    private readonly LemmingTracker _lemmingTracker;
    private readonly HitBoxGadgetState[] _states;

    /// <summary>
    /// This property refers to the positions of the hitboxes, not the gadget itself
    /// </summary>
    public RectangularRegion CurrentBounds => CurrentState.GetMininmumBoundingBoxForAllHitBoxes(CurrentGadgetBounds.Position);

    public ResizeType ResizeType { get; }

    public HitBoxGadget(
        HitBoxGadgetState[] states,
        LemmingTracker lemmingTracker,
        int initialStateIndex,
        ResizeType resizeType)
        : base(Common.Enums.GadgetType.HitBoxGadget)
    {
        _states = states;
        _lemmingTracker = lemmingTracker;
        StateIndex = initialStateIndex;
        ResizeType = resizeType;

        foreach (var state in _states)
        {
            state.SetParentGadget(this);
        }
    }

    public override HitBoxGadgetState CurrentState => _states[StateIndex];

    public override void Tick()
    {
        CurrentState.Tick();
    }

    public override void SetState(int stateIndex)
    {
        StateIndex = stateIndex;

        // Changing states may change hitbox positions 
        // Force a position update to accommodate this
        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
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
        var gadgetManager = LevelScreen.GadgetManager;

        var onHitBehaviours = activeFilter.OnLemmingHitBehaviours;
        foreach (var gadgetBehaviour in onHitBehaviours)
        {
            gadgetManager.RegisterCauseAndEffectData(gadgetBehaviour, lemming);
        }

        var lemmingTrackingHitBoxBehaviours = GetLemmingTrackingHitBoxBehaviours(activeFilter, lemming);
        foreach (var gadgetBehaviour in lemmingTrackingHitBoxBehaviours)
        {
            gadgetManager.RegisterCauseAndEffectData(gadgetBehaviour, lemming);
        }
    }

    private ReadOnlySpan<GadgetBehaviour> GetLemmingTrackingHitBoxBehaviours(
        LemmingHitBoxFilter activeFilter,
        Lemming lemming)
    {
        var trackingStatus = _lemmingTracker.UpdateLemmingTrackingStatus(lemming);

        return trackingStatus switch
        {
            TrackingStatus.Absent => ReadOnlySpan<GadgetBehaviour>.Empty,
            TrackingStatus.Entered => activeFilter.OnLemmingEnterBehaviours,
            TrackingStatus.Exited => activeFilter.OnLemmingExitBehaviours,
            TrackingStatus.StillPresent => activeFilter.OnLemmingPresentBehaviours,

            _ => ReadOnlySpan<GadgetBehaviour>.Empty
        };
    }

    public void Move(Point delta)
    {
        CurrentGadgetBounds.Position = LevelScreen.NormalisePosition(CurrentGadgetBounds.Position + delta);
        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public void SetPosition(Point position)
    {
        CurrentGadgetBounds.Position = LevelScreen.NormalisePosition(position);
        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public void Resize(int dw, int dh)
    {
        var r = ResizeType;
        int tempSize;

        if (r.CanResizeHorizontally())
        {
            tempSize = CurrentGadgetBounds.Width;
            tempSize += dw;
            if (tempSize < 0) tempSize = 0;
            if (tempSize > EngineConstants.MaxLevelSize) tempSize = EngineConstants.MaxLevelSize;
            CurrentGadgetBounds.Width = tempSize;
        }

        if (r.CanResizeVertically())
        {
            tempSize = CurrentGadgetBounds.Height;
            tempSize += dh;
            if (tempSize < 0) tempSize = 0;
            if (tempSize > EngineConstants.MaxLevelSize) tempSize = EngineConstants.MaxLevelSize;
            CurrentGadgetBounds.Height = tempSize;
        }

        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public void SetSize(int w, int h)
    {
        var r = ResizeType;

        if (r.CanResizeHorizontally())
        {
            if (w < 0) w = 0;
            if (w > EngineConstants.MaxLevelSize) w = EngineConstants.MaxLevelSize;
            CurrentGadgetBounds.Width = w;
        }

        if (r.CanResizeVertically())
        {
            if (h < 0) h = 0;
            if (h > EngineConstants.MaxLevelSize) h = EngineConstants.MaxLevelSize;
            CurrentGadgetBounds.Height = h;
        }

        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public bool Equals(HitBoxGadget? other)
    {
        var otherId = -1;
        if (other is not null) otherId = other.Id;
        return Id == otherId;
    }

    public static bool operator ==(HitBoxGadget? left, HitBoxGadget? right)
    {
        var leftId = -1;
        if (left is not null) leftId = left.Id;
        var rightId = -1;
        if (right is not null) rightId = right.Id;
        return leftId == rightId;
    }
    public static bool operator !=(HitBoxGadget? left, HitBoxGadget? right) => !(left == right);
}
