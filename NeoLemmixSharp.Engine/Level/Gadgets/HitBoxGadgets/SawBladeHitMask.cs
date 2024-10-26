using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class SawBladeHitMask
{
    private readonly TerrainEraseMask _terrainEraseMask;
    private readonly PointSetHitBoxRegion _hitBox;

    public SawBladeHitMask(
        IDestructionMask destructionMask,
        LevelPosition[] mask)
    {
        _terrainEraseMask = new TerrainEraseMask(destructionMask, new LevelPosition(0, 0), mask);
        _hitBox = new PointSetHitBoxRegion(mask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MatchesPosition(LevelPosition levelPosition) => _hitBox.ContainsPoint(levelPosition);

    public void ApplyEraseMask(LevelPosition levelPosition)
    {
        _terrainEraseMask.ApplyEraseMask(DownOrientation.Instance, FacingDirection.RightInstance, levelPosition);
    }
}