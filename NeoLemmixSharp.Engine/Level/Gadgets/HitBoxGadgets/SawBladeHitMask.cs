using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class SawBladeHitMask
{
    private readonly RectangularLevelRegion _bounds;
    private readonly TerrainEraseMask _terrainEraseMask;
    private readonly PointSetLevelRegion _hitBox;

    public SawBladeHitMask(
        IDestructionMask destructionMask,
        RectangularLevelRegion bounds,
        LevelPosition[] mask)
    {
        _bounds = bounds;
        _terrainEraseMask = new TerrainEraseMask(destructionMask, new LevelPosition(0, 0), mask);
        _hitBox = new PointSetLevelRegion(mask);
    }

    public bool MatchesPosition(LevelPosition levelPosition)
    {
        levelPosition -= _bounds.TopLeft;

        return _hitBox.ContainsPoint(levelPosition);
    }

    public void ApplyEraseMask(LevelPosition levelPosition)
    {
        _terrainEraseMask.ApplyEraseMask(DownOrientation.Instance, RightFacingDirection.Instance, levelPosition);
    }
}