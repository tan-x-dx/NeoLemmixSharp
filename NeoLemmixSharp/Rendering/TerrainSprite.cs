using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using System;

namespace NeoLemmixSharp.Rendering;

public sealed class TerrainSprite : IDisposable
{
    private readonly Texture2D _texture;

    private readonly uint[] _colourSetter = new uint[1];

    public TerrainSprite(Texture2D texture)
    {
        _texture = texture;
    }

    public void Render(SpriteBatch spriteBatch)
    {
        var viewport = LevelScreen.CurrentLevel.Viewport;
        var maxX = viewport.NumberOfHorizontalRenderIntervals;
        var maxY = viewport.NumberOfVerticalRenderIntervals;

        for (var i = 0; i < maxX; i++)
        {
            var hInterval = viewport.GetHorizontalRenderInterval(i);
            for (var j = 0; j < maxY; j++)
            {
                var vInterval = viewport.GetVerticalRenderInterval(j);
                var sourceRect = new Rectangle(hInterval.PixelStart, vInterval.PixelStart, hInterval.PixelLength, vInterval.PixelLength);
                var screenRect = new Rectangle(hInterval.ScreenStart, vInterval.ScreenStart, hInterval.ScreenLength, vInterval.ScreenLength);

                spriteBatch.Draw(_texture, screenRect, sourceRect, Color.White);
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