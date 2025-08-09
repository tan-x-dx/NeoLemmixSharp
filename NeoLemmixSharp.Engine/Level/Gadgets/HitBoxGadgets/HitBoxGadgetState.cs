using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class HitBoxGadgetState : GadgetState
{
    private HitBoxGadget _parentGadget = null!;

    private readonly LemmingHitBoxFilter[] _lemmingHitBoxFilters;
    private readonly BitArrayDictionary<Orientation.OrientationHasher, BitBuffer32, Orientation, HitBoxRegion> _hitBoxLookup;

    public ReadOnlySpan<LemmingHitBoxFilter> Filters => new(_lemmingHitBoxFilters);

    public HitBoxGadgetState(
        GadgetTrigger[] gadgetTriggers,
        LemmingHitBoxFilter[] lemmingHitBoxFilters,
        BitArrayDictionary<Orientation.OrientationHasher, BitBuffer32, Orientation, HitBoxRegion> hitBoxLookup)
        : base(gadgetTriggers)
    {
        _lemmingHitBoxFilters = lemmingHitBoxFilters;
        _hitBoxLookup = hitBoxLookup;
    }

    public void SetParentGadget(HitBoxGadget hitBoxGadget)
    {
        if (_parentGadget is not null)
            throw new InvalidOperationException("Cannot set parent gadget more than once!");

        _parentGadget = hitBoxGadget;
    }

    public HitBoxRegion HitBoxFor(Orientation orientation)
    {
        if (_hitBoxLookup.TryGetValue(orientation, out var hitBoxRegion))
            return hitBoxRegion;
        return EmptyHitBoxRegion.Instance;
    }

    public RectangularRegion GetMininmumBoundingBoxForAllHitBoxes(Point offset)
    {
        if (_hitBoxLookup.Count == 0)
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

    protected override void OnTick()
    {
        foreach (var lemmingHitBoxFilter in _lemmingHitBoxFilters)
        {
            lemmingHitBoxFilter.Tick();
        }
    }

    public override void OnTransitionTo()
    {
        //  StateSelectedOutput.SetSignal(true);
    }

    public override void OnTransitionFrom()
    {
        // StateSelectedOutput.SetSignal(false);
    }

    public override GadgetRenderer Renderer => throw new NotImplementedException();
}
