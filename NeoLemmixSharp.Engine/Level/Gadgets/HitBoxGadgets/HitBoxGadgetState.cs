using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets.Animations;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Diagnostics;
using OrientationToHitBoxRegionLookup = NeoLemmixSharp.Common.Util.Collections.BitArrays.BitArrayDictionary<NeoLemmixSharp.Common.Orientation.OrientationHasher, NeoLemmixSharp.Common.Util.Collections.BitArrays.BitBuffer32, NeoLemmixSharp.Common.Orientation, NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes.IHitBoxRegion>;


namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class HitBoxGadgetState : GadgetState
{
    private HitBoxGadget _parentGadget = null!;
    private readonly GadgetStateName _stateName;

    private readonly LemmingHitBoxFilter[] _lemmingHitBoxFilters;
    private readonly OrientationToHitBoxRegionLookup? _hitBoxLookup;

    public AnimationController AnimationController { get; }
    public ReadOnlySpan<LemmingHitBoxFilter> Filters => new(_lemmingHitBoxFilters);

    public HitBoxGadgetState(
        GadgetStateName stateName,
        AnimationController animationController)
    {
        _stateName = stateName;
        _lemmingHitBoxFilters = [];
        _hitBoxLookup = null;
        AnimationController = animationController;
    }

    public HitBoxGadgetState(
        GadgetStateName stateName,
        LemmingHitBoxFilter[] lemmingHitBoxFilters,
        OrientationToHitBoxRegionLookup hitBoxLookup,
        AnimationController animationController)
    {
        _stateName = stateName;
        _lemmingHitBoxFilters = lemmingHitBoxFilters;
        _hitBoxLookup = hitBoxLookup;
        AnimationController = animationController;
    }

    public void SetParentGadget(HitBoxGadget hitBoxGadget)
    {
        if (_parentGadget is not null)
            throw new InvalidOperationException("Cannot set parent gadget more than once!");

        _parentGadget = hitBoxGadget;
    }

    public IHitBoxRegion HitBoxFor(Orientation orientation)
    {
        Debug.Assert(_hitBoxLookup != null);

        if (_hitBoxLookup!.TryGetValue(orientation, out var hitBoxRegion))
            return hitBoxRegion;
        return EmptyHitBoxRegion.Instance;
    }

    public RectangularRegion GetMininmumBoundingBoxForAllHitBoxes(Point offset)
    {
        Debug.Assert(_hitBoxLookup != null);

        if (_hitBoxLookup!.Count == 0)
            return new RectangularRegion(offset);

        var x = int.MaxValue;
        var y = int.MaxValue;
        var w = int.MinValue;
        var h = int.MinValue;

        foreach (var kvp in _hitBoxLookup)
        {
            var hitBoxBounds = kvp.Value.CurrentBounds;
            var bottomRight = hitBoxBounds.GetBottomRight();

            x = Math.Min(x, hitBoxBounds.X);
            y = Math.Min(y, hitBoxBounds.Y);
            w = Math.Max(w, bottomRight.X);
            h = Math.Max(h, bottomRight.Y);
        }

        w += 1 - x;
        h += 1 - y;

        x += offset.X;
        y += offset.Y;

        return new RectangularRegion(new Point(x, y), new Size(w, h));
    }

    public override void OnTransitionTo()
    {
        AnimationController.OnTransitionTo();
        //  StateSelectedOutput.SetSignal(true);
    }

    public override void Tick()
    {
        AnimationController.Tick();

        var nextStateIndex = AnimationController.GetNextStateIndex();
        if (nextStateIndex >= 0)
        {
            _parentGadget.SetNextState(nextStateIndex);
        }
    }

    public override void OnTransitionFrom()
    {
        // StateSelectedOutput.SetSignal(false);
    }

    public override string ToString() => _stateName.ToString();

    public override GadgetRenderer Renderer { get; }
}
