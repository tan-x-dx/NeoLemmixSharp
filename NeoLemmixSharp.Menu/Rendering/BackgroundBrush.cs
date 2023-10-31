using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class BackgroundBrush : IBrush
{
    private readonly Texture2D _texture;
    private readonly TextureRegion _textureRegion;

    public BackgroundBrush(Texture2D texture)
    {
        _texture = texture;
        _textureRegion = new TextureRegion(texture);
    }

    public void Draw(RenderContext context, Rectangle dest, Color color)
    {
        var numX = 1 + dest.Width / _texture.Width;
        var numY = 1 + dest.Height / _texture.Height;

        for (var x = 0; x < numX; x++)
        {
            var vx = x * _texture.Width;

            for (var y = 0; y < numY; y++)
            {
                var vy = y * _texture.Height;

                _textureRegion.Draw(context, new Rectangle(vx, vy, _texture.Width, _texture.Height), Color.White);
            }
        }
    }
}