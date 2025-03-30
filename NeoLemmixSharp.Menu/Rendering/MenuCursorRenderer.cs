using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Rendering;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class MenuCursorRenderer
{
    private readonly MenuInputController _menuInputController;
    private readonly Texture2D _cursorTexture;

    public MenuCursorRenderer(MenuInputController menuInputController)
    {
        _menuInputController = menuInputController;
        _cursorTexture = CommonSprites.CursorHandHiRes;
    }

    public void RenderCursor(SpriteBatch spriteBatch)
    {
        var mousePosition = _menuInputController.MousePosition;
        var destination = new Rectangle(
            mousePosition.X + CommonSprites.CursorHiResXOffset,
            mousePosition.Y + CommonSprites.CursorHiResYOffset,
            CommonSprites.CursorHiResWidth * 2,
            CommonSprites.CursorHiResHeight * 2);

        var source = new Rectangle(
            0,
            0,
            CommonSprites.CursorHiResWidth,
            CommonSprites.CursorHiResHeight);

        spriteBatch.Draw(
            _cursorTexture,
            destination,
            source,
            Color.White);

        source.X = CommonSprites.CursorHiResWidth;

        spriteBatch.Draw(
            _cursorTexture,
            destination,
            source,
            new Color(0x88, 0x88, 0x22));
    }
}