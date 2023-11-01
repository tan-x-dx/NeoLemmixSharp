using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class MenuCursorRenderer : IDisposable
{
    private readonly MenuInputController _menuCursor;
    private readonly Texture2D _cursorTexture;

    public MenuCursorRenderer(MenuSpriteBank menuSpriteBank, MenuInputController menuCursor)
    {
        _menuCursor = menuCursor;
        _cursorTexture = menuSpriteBank.GetTexture(MenuResource.Cursor);
    }

    public void RenderCursor(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_cursorTexture, new Vector2(_menuCursor.MouseX, _menuCursor.MouseY), Color.White);
    }

    public void Dispose()
    {
    }
}