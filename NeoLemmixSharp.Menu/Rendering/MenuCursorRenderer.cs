using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class MenuCursorRenderer : IDisposable
{
    private readonly MenuInputController _menuCursor;
    private Texture2D _cursorTexture;

    public MenuCursorRenderer(GraphicsDevice graphicsDevice, MenuInputController menuCursor)
    {
        _menuCursor = menuCursor;
        _cursorTexture = CreateCursorTexture_Debug(graphicsDevice);
    }

    private Texture2D CreateCursorTexture_Debug(GraphicsDevice graphicsDevice)
    {
        var cursorTexture = new Texture2D(graphicsDevice, 32, 32);

        var white = Color.White.PackedValue;
        var red = new Color(200, 0, 0, 255).PackedValue;
        var blue = new Color(200, 0, 200, 255).PackedValue;

        var colors = new uint[32 * 32];
        for (var x = 0; x < 32; x++)
        {
            for (var y = 0; y < 32; y++)
            {
                if (x < y + y)
                {
                    colors[y * 32 + x] = white;
                }

                if (x == y + y)
                {
                    colors[y * 32 + x] = blue;
                }

                if (x + x == y)
                {
                    colors[y * 32 + x] = red;
                }

                if (x + x < y)
                {
                    colors[y * 32 + x] = 0U;
                }
            }
        }

        for (var x = 16; x < 32; x++)
        {
            for (var y = 16; y < 32; y++)
            {
                colors[y * 32 + x] = 0U;
            }
        }

        cursorTexture.SetData(colors);

        return cursorTexture;
    }

    public void Dispose()
    {
        HelperMethods.DisposeOf(ref _cursorTexture);
    }

    public void RenderCursor(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_cursorTexture, new Vector2(_menuCursor.MouseX, _menuCursor.MouseY), Color.White);
    }
}