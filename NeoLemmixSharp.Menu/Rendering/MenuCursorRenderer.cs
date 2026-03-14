using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util.GameInput;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class MenuCursorRenderer
{
    private readonly InputHandler _inputHandler;
    private readonly Texture2D _cursorTexture;

    public MenuCursorRenderer(InputHandler menuInputController)
    {
        _inputHandler = menuInputController;
        _cursorTexture = CommonSprites.CursorHandHiRes;
    }

    public void RenderCursor(SpriteBatch spriteBatch)
    {
        var mousePosition = _inputHandler.MousePosition;
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
