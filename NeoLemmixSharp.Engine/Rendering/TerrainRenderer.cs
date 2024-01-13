using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;

namespace NeoLemmixSharp.Engine.Rendering;

public sealed class TerrainRenderer : IDisposable
{
    private readonly Texture2D _texture;
    private readonly Level.Viewport _viewport;

    private readonly uint[] _colorSetter = new uint[1];

    public TerrainRenderer(
        Texture2D texture,
        Level.Viewport viewport)
    {
        _texture = texture;
        _viewport = viewport;
    }

    public void RenderTerrain(SpriteBatch spriteBatch)
    {
        var maxX = _viewport.NumberOfHorizontalRenderIntervals;
        var maxY = _viewport.NumberOfVerticalRenderIntervals;

        for (var i = 0; i < maxX; i++)
        {
            var hInterval = _viewport.GetHorizontalRenderInterval(i);
            for (var j = 0; j < maxY; j++)
            {
                var vInterval = _viewport.GetVerticalRenderInterval(j);
                var destinationRectangle = new Rectangle(hInterval.ScreenStart, vInterval.ScreenStart, hInterval.PixelLength, vInterval.PixelLength);
                var sourceRectangle = new Rectangle(hInterval.PixelStart, vInterval.PixelStart, hInterval.PixelLength, vInterval.PixelLength);

                spriteBatch.Draw(
                    _texture,
                    destinationRectangle,
                    sourceRectangle,
                    RenderingLayers.TerrainLayer);
            }
        }
    }

    public void SetPixelColor(int x, int y, uint color)
    {
        _colorSetter[0] = color;
        _texture.SetData(0, new Rectangle(x, y, 1, 1), _colorSetter, 0, 1);
    }

    public void Dispose()
    {
        _texture.Dispose();
    }
}