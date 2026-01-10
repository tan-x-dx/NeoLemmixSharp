using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.Movement;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
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

    // The below properties refer to the positions of the hitboxes, not the gadget itself
    public RectangularRegion CurrentBounds => CurrentState.GetMininmumBoundingBoxForAllHitBoxes(CurrentGadgetBounds.Position);

    public ResizeType ResizeType { get; }

    public HitBoxGadget(
        HitBoxGadgetState[] states,
        int initialStateIndex,
        ResizeType resizeType,
        LemmingTracker lemmingTracker)
        : base(Common.Enums.GadgetType.HitBoxGadget)
    {
        _lemmingTracker = lemmingTracker;
        _states = states;

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
        _lemmingTracker.Tick();

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

        var lemmingBehaviours = activeFilter.OnLemmingHitBehaviours;
        foreach (var lemmingBehaviour in lemmingBehaviours)
        {
            gadgetManager.RegisterCauseAndEffectData(lemmingBehaviour, lemming.Id);
        }

        var hitBoxLemmingBehaviours = GetHitBoxLemmingBehaviours(activeFilter, lemming);
        foreach (var lemmingBehaviour in hitBoxLemmingBehaviours)
        {
            gadgetManager.RegisterCauseAndEffectData(lemmingBehaviour, lemming.Id);
        }
    }

    private ReadOnlySpan<GadgetBehaviour> GetHitBoxLemmingBehaviours(
        LemmingHitBoxFilter activeFilter,
        Lemming lemming)
    {
        var trackingStatus = _lemmingTracker.TrackItem(lemming);

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
        if (ResizeType == ResizeType.None)
            return;

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

        if (ResizeType.CanResizeHorizontally())
            CurrentGadgetBounds.Width = w;

        if (ResizeType.CanResizeVertically())
            CurrentGadgetBounds.Height = h;

        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public bool Equals(HitBoxGadget? other)
    {
        var otherValue = -1;
        if (other is not null) otherValue = other.Id;
        return Id == otherValue;
    }

    public static bool operator ==(HitBoxGadget? left, HitBoxGadget? right)
    {
        var leftValue = -1;
        if (left is not null) leftValue = left.Id;
        var rightValue = -1;
        if (right is not null) rightValue = right.Id;
        return leftValue == rightValue;
    }
    public static bool operator !=(HitBoxGadget? left, HitBoxGadget? right) => !(left == right);
}
