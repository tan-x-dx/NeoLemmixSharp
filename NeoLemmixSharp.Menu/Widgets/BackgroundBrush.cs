using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;

namespace NeoLemmixSharp.Menu.Widgets;

public sealed class BackgroundBrush : IBrush
{
    private readonly Texture2D _backgroundTexture;
    private readonly TextureRegion _textureRegion;

    private int _backgroundTileX;
    private int _backgroundTileY;

    public BackgroundBrush(Texture2D backgroundTexture)
    {
        _backgroundTexture = backgroundTexture;
        _textureRegion = new TextureRegion(backgroundTexture);
    }

    public void SetWindowDimensions(int windowWidth, int windowHeight)
    {
        var textureWidth = _backgroundTexture.Width;
        var textureHeight = _backgroundTexture.Height;

        _backgroundTileX = (int)Math.Ceiling((double)windowWidth / textureWidth);
        _backgroundTileY = (int)Math.Ceiling((double)windowHeight / textureHeight);
    }

    public void Draw(RenderContext context, Rectangle dest, Color color)
    {
        for (var x = 0; x < _backgroundTileX; x++)
        {
            var vx = x * _backgroundTexture.Width;

            for (var y = 0; y < _backgroundTileY; y++)
            {
                var vy = y * _backgroundTexture.Height;

                _textureRegion.Draw(
                    context,
                    new Rectangle(vx, vy, _backgroundTexture.Width, _backgroundTexture.Height),
                    Color.White);
            }
        }
    }
}