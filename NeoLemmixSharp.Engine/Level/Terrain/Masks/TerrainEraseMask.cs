using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Terrain.Masks;

public sealed class TerrainEraseMask
{
#pragma warning disable CS8618
    private static TerrainManager _terrainManager;
#pragma warning restore CS8618

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

        for (var i = 0; i < _mask.Length; i++)
        {
            var pixel = _mask[i];

            pixel = _terrainManager.NormalisePosition(pixel + offset);

            _terrainManager.ErasePixel(orientation, _destructionMask, facingDirection, pixel);
        }
    }

    public static void SetTerrainManager(TerrainManager manager)
    {
        _terrainManager = manager;
    }
}