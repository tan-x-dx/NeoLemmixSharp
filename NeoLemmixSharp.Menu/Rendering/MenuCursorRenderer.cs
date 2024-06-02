using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Screen;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class MenuCursorRenderer
{
    private readonly MenuInputController _menuInputController;
    private readonly Texture2D _cursorTexture;

    public MenuCursorRenderer(MenuInputController menuInputController)
    {
        _menuInputController = menuInputController;
        _cursorTexture = MenuSpriteBank.CursorHiRes;
    }

    public void RenderCursor(SpriteBatch spriteBatch)
    {
        var destination = new Rectangle(
            _menuInputController.MouseX + MenuSpriteBank.CursorHiResXOffset,
            _menuInputController.MouseY + MenuSpriteBank.CursorHiResYOffset,
            MenuSpriteBank.CursorHiResWidth * 2,
            MenuSpriteBank.CursorHiResHeight * 2);

        var source = new Rectangle(
            0,
            0,
            MenuSpriteBank.CursorHiResWidth,
            MenuSpriteBank.CursorHiResHeight);

        spriteBatch.Draw(
            _cursorTexture,
            destination,
            source,
            Color.White);

        source.X = MenuSpriteBank.CursorHiResWidth;

        spriteBatch.Draw(
            _cursorTexture,
            destination,
            source,
            new Color(0x88, 0x88, 0x22));
    }
}