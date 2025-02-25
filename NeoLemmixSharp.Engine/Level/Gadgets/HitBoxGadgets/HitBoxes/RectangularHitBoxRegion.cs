using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public sealed class RectangularHitBoxRegion : IHitBoxRegion
{
    private readonly LevelRegion _region;

    public LevelRegion CurrentBounds => _region;

    public RectangularHitBoxRegion(
        int x,
        int y,
        int w,
        int h)
    {
        var position = new LevelPosition(x, y);
        var size = new LevelSize(w, h);
        _region = new LevelRegion(position, size);
    }

    public RectangularHitBoxRegion(
        LevelPosition p0,
        LevelPosition p1)
    {
        _region = new LevelRegion(p0, p1);
    }

    public bool ContainsPoint(LevelPosition levelPosition)
    {
        return LevelScreen.RegionContainsPoint(_region, levelPosition);
    }
}
