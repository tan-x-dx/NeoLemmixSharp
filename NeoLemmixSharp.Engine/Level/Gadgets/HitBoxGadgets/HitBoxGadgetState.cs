using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class HitBoxGadgetState : GadgetState
{
    private readonly LemmingHitBoxFilter[] _lemmingHitBoxFilters;
    private readonly BitArrayDictionary<Orientation.OrientationHasher, BitBuffer32, Orientation, HitBoxRegion> _hitBoxLookup;

    public ReadOnlySpan<LemmingHitBoxFilter> Filters => new(_lemmingHitBoxFilters);

    public HitBoxGadgetState(
        LemmingHitBoxFilter[] lemmingHitBoxFilters,
        BitArrayDictionary<Orientation.OrientationHasher, BitBuffer32, Orientation, HitBoxRegion> hitBoxLookup)
    {
        _lemmingHitBoxFilters = lemmingHitBoxFilters;
        _hitBoxLookup = hitBoxLookup;
    }

    public HitBoxRegion HitBoxFor(Orientation orientation)
    {
        if (_hitBoxLookup.TryGetValue(orientation, out var hitBoxRegion))
            return hitBoxRegion;
        return EmptyHitBoxRegion.Instance;
    }

    protected override void OnSetParentGadget()
    {
        foreach (var filter in _lemmingHitBoxFilters)
        {
            filter.SetParentData(ParentGadget, this);
        }
    }

    [SkipLocalsInit]
    public unsafe RectangularRegion GetMininmumBoundingBoxForAllHitBoxes(Point offset)
    {
        if (_hitBoxLookup.Count == 0)
            goto TrivialBounds;

        RectangularRegion* pRegions = stackalloc RectangularRegion[OrientationConstants.NumberOfOrientations];
        var i = 0;

        foreach (var kvp in _hitBoxLookup)
        {
            var hitBox = kvp.Value;
            if (hitBox.IsTrivial())
                continue;

            pRegions[i++] = hitBox.CurrentBounds;
        }

        if (i == 0)
            goto TrivialBounds;

        i <<= 1;

        var pointsSpan = Helpers.CreateReadOnlySpan<Point>(pRegions, i);
        var result = new RectangularRegion(pointsSpan);

        return result.Translate(offset);

    TrivialBounds:
        return new RectangularRegion(offset);
    }

    public override GadgetRenderer Renderer => throw new NotImplementedException();
}
