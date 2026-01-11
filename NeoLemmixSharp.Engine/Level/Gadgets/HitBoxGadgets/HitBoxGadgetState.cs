using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

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

    public RectangularRegion GetMininmumBoundingBoxForAllHitBoxes(Point offset)
    {
        if (_hitBoxLookup.Count == 0)
            return new RectangularRegion(offset);

        var minX = int.MaxValue;
        var minY = int.MaxValue;
        var maxX = int.MinValue;
        var maxY = int.MinValue;

        foreach (var kvp in _hitBoxLookup)
        {
            var hitBoxBounds = kvp.Value.CurrentBounds;
            var bottomRight = hitBoxBounds.GetBottomRight();

            if (hitBoxBounds.X < minX)
                minX = hitBoxBounds.X;
            if (hitBoxBounds.Y < minY)
                minY = hitBoxBounds.Y;
            if (bottomRight.X > maxX)
                maxX = bottomRight.X;
            if (bottomRight.Y > maxY)
                maxY = bottomRight.Y;
        }

        minX += offset.X;
        minY += offset.Y;
        maxX += offset.X;
        maxY += offset.Y;

        return new RectangularRegion(new Point(minX, minY), new Point(maxX, maxY));
    }

    public override GadgetRenderer Renderer => throw new NotImplementedException();
}
