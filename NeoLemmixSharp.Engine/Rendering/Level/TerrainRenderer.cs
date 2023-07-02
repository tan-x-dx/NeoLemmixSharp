using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Engine;

namespace NeoLemmixSharp.Engine.Rendering.Level;

public sealed class TerrainRenderer : IDisposable
{
    private readonly Texture2D _texture;
    private readonly LevelViewport _viewport;

    private readonly uint[] _colourSetter = new uint[1];

    public TerrainRenderer(
        Texture2D texture,
        LevelViewport viewport)
    {
        _texture = texture;
        _viewport = viewport;
    }

    public void Render(SpriteBatch spriteBatch)
    {
        var maxX = _viewport.NumberOfHorizontalRenderIntervals;
        var maxY = _viewport.NumberOfVerticalRenderIntervals;

        for (var i = 0; i < maxX; i++)
        {
            var hInterval = _viewport.GetHorizontalRenderInterval(i);
            for (var j = 0; j < maxY; j++)
            {
                var vInterval = _viewport.GetVerticalRenderInterval(j);
                var sourceRect = new Rectangle(hInterval.PixelStart, vInterval.PixelStart, hInterval.PixelLength, vInterval.PixelLength);
                var screenRect = new Rectangle(hInterval.ScreenStart, vInterval.ScreenStart, hInterval.ScreenLength, vInterval.ScreenLength);

                spriteBatch.Draw(
                    _texture,
                    screenRect,
                    sourceRect,
                    Color.White,
                    0.0f,
                    new Vector2(),
                    SpriteEffects.None,
                    RenderingLayers.TerrainLayer);
            }
        }
    }

    public void SetPixelColour(int x, int y, uint colour)
    {
        _colourSetter[0] = colour;
        _texture.SetData(0, new Rectangle(x, y, 1, 1), _colourSetter, 0, 1);
    }

    public void Dispose()
    {
        _texture.Dispose();
    }
}