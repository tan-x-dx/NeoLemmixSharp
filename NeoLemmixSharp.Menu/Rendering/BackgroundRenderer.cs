using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class BackgroundRenderer
{
    private readonly Texture2D _texture;

    private int _tileX;
    private int _tileY;

    public BackgroundRenderer(Texture2D texture)
    {
        _texture = texture;
    }

    public void SetWindowDimensions(int windowWidth, int windowHeight)
    {
        _tileX = 1 + windowWidth / _texture.Width;
        _tileY = 1 + windowHeight / _texture.Height;
    }

    public void Render(SpriteBatch spriteBatch)
    {
        for (var x = 0; x < _tileX; x++)
        {
            var vx = x * _texture.Width;

            for (var y = 0; y < _tileY; y++)
            {
                var vy = y * _texture.Height;

                spriteBatch.Draw(_texture, new Vector2(vx, vy), Color.White);
            }
        }
    }
}