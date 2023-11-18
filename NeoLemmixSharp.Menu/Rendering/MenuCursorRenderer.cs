using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class MenuCursorRenderer : IDisposable
{
    private readonly MenuInputController _menuCursor;
    private readonly Texture2D _cursorTexture;

    public MenuCursorRenderer(MenuInputController menuCursor)
    {
        _menuCursor = menuCursor;
        _cursorTexture = MenuSpriteBank.GetTexture(MenuResource.Cursor);
    }

    public void RenderCursor(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            _cursorTexture,
            new Vector2(_menuCursor.MouseX, _menuCursor.MouseY),
            Color.White,
            1f);
    }

    public void Dispose()
    {
    }
}