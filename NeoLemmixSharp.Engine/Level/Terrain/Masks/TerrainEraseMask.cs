using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Terrain.Masks;

public sealed class TerrainEraseMask
{
    private readonly IDestructionMask _destructionMask;
    private readonly LevelPosition _anchorPoint;
    private readonly LevelPosition[] _mask;

    public TerrainEraseMask(
        IDestructionMask destructionMask,
        LevelPosition anchorPoint,
        LevelPosition[] mask)
    {
        _destructionMask = destructionMask;
        _anchorPoint = anchorPoint;
        _mask = mask;
    }

    public void ApplyEraseMask(
        Orientation orientation,
        FacingDirection facingDirection,
        LevelPosition position)
    {
        var offset = position - _anchorPoint;
        var terrainManager = Global.TerrainManager;

        for (var i = 0; i < _mask.Length; i++)
        {
            var pixel = _mask[i];

            pixel = terrainManager.NormalisePosition(pixel + offset);

            terrainManager.ErasePixel(orientation, _destructionMask, facingDirection, pixel);
        }
    }
}