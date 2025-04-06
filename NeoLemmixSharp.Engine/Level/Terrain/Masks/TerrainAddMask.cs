using NeoLemmixSharp.Common;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.Engine.Level.Terrain.Masks;

public sealed class TerrainAddMask
{
    private readonly Point _anchorPoint;
    private readonly Point[] _mask;
    private readonly Color[] _colorMask;

    public TerrainAddMask(
        Point anchorPoint,
        Point[] mask,
        Color[] colorMask)
    {
        _anchorPoint = anchorPoint;
        _mask = mask;
        _colorMask = colorMask;
    }

    public void ApplyAddMask(Point position)
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