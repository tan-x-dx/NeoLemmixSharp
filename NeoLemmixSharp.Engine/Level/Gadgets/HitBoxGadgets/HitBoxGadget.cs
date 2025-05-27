using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
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
    // The below properties refer to the positions of the hitboxes, not the gadget itself
    public RectangularRegion CurrentBounds => CurrentState.GetMininmumBoundingBoxForAllHitBoxes(CurrentGadgetBounds.Position);

    public ResizeType ResizeType { get; }

    public HitBoxGadget(
        string gadgetName,
        GadgetState[] states,
        int initialStateIndex,
        ResizeType resizeType,
        LemmingTracker lemmingTracker)
        : base(gadgetName, states, initialStateIndex)
    {
        _lemmingTracker = lemmingTracker;

        ResizeType = resizeType;
    }

    protected override void OnTick()
    {
        _lemmingTracker.Tick();
    }

    protected override void OnChangeStates()
    {
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
        var actionsToPerform = GetActionsToPerformOnLemming(activeFilter, lemming);

        for (var i = 0; i < actionsToPerform.Length; i++)
        {
            actionsToPerform[i].PerformAction(lemming);
        }
    }

    private ReadOnlySpan<GadgetAction> GetActionsToPerformOnLemming(
        LemmingHitBoxFilter activeFilter,
        Lemming lemming)
    {
        var trackingStatus = _lemmingTracker.TrackItem(lemming);

        return trackingStatus switch
        {
            TrackingStatus.Absent => ReadOnlySpan<GadgetAction>.Empty,
            TrackingStatus.JustAdded => activeFilter.OnLemmingEnterActions,
            TrackingStatus.JustRemoved => activeFilter.OnLemmingExitActions,
            TrackingStatus.StillPresent => activeFilter.OnLemmingPresentActions,

            _ => ReadOnlySpan<GadgetAction>.Empty
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
