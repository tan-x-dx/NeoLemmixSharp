using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Terrain.Masks;

public sealed class TerrainAddMask
{
    private readonly LevelPosition _anchorPoint;
    private readonly LevelPosition[] _mask;
    private readonly uint[] _colorMask;

    public TerrainAddMask(
        LevelPosition anchorPoint,
        LevelPosition[] mask,
        uint[] colorMask)
    {
        _anchorPoint = anchorPoint;
        _mask = mask;
        _colorMask = colorMask;
    }

    public void ApplyAddMask(LevelPosition position)
    {
        var offset = position - _anchorPoint;
        var terrainManager = LevelScreen.TerrainManager;

        for (var i = 0; i < _mask.Length; i++)
        {
            var pixel = _mask[i];
            var color = _colorMask[i];

            pixel += offset;

            terrainManager.SetSolidPixel(pixel, color);
        }
    }
}