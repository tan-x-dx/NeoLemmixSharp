using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.Level;

namespace NeoLemmixSharp.Engine.Rendering.Viewport;

public sealed class LevelCursorSprite
{
    private readonly LevelCursor _levelCursor;
    private readonly Texture2D _cursorTexture;
    private readonly Texture2D _handTexture;

    public LevelCursorSprite(
        LevelCursor levelCursor,
        Texture2D cursorTexture,
        Texture2D handTexture)
    {
        _levelCursor = levelCursor;
        _cursorTexture = cursorTexture;
        _handTexture = handTexture;
    }

    public void RenderAtPosition(SpriteBatch spriteBatch, int x, int y, int scaleMultiplier)
    {
        if (LevelScreen.LevelViewport.MouseIsInLevelViewPort)
        {
            RenderCursor(spriteBatch, x, y, scaleMultiplier);
        }
        else
        {
            RenderHand(spriteBatch, x, y);
        }
    }

    private void RenderCursor(SpriteBatch spriteBatch, int x, int y, int scaleMultiplier)
    {
        var cursorSize = EngineConstants.HalfCursorSizeInPixels * scaleMultiplier;

        x -= cursorSize;
        y -= cursorSize;
        cursorSize *= 2;

        var destRectangle = new Rectangle(
            x,
            y,
            cursorSize,
            cursorSize);

        var sourceRect = new Rectangle(0, 0, EngineConstants.CursorSizeInPixels, EngineConstants.CursorSizeInPixels);
        var sourceY = _levelCursor.NumberOfLemmingsUnderCursor > 0
            ? 1
            : 0;

        sourceY *= EngineConstants.CursorSizeInPixels;

        sourceRect.Y = sourceY;

        spriteBatch.Draw(
            _cursorTexture,
            destRectangle,
            sourceRect,
            _levelCursor.Color1);

        sourceRect.X += EngineConstants.CursorSizeInPixels;

        spriteBatch.Draw(
            _cursorTexture,
            destRectangle,
            sourceRect,
            _levelCursor.Color2);

        sourceRect.X += EngineConstants.CursorSizeInPixels;

        spriteBatch.Draw(
            _cursorTexture,
            destRectangle,
            sourceRect,
            _levelCursor.Color3);
    }

    private void RenderHand(SpriteBatch spriteBatch, int x, int y)
    {
        var destination = new Rectangle(
            x + CommonSprites.CursorHiResXOffset,
            y + CommonSprites.CursorHiResYOffset,
            CommonSprites.CursorHiResWidth * 2,
            CommonSprites.CursorHiResHeight * 2);

        var source = new Rectangle(
            0,
            0,
            CommonSprites.CursorHiResWidth,
            CommonSprites.CursorHiResHeight);

        spriteBatch.Draw(
            _handTexture,
            destination,
            source,
            Color.White);

        source.X = CommonSprites.CursorHiResWidth;

        spriteBatch.Draw(
            _handTexture,
            destination,
            source,
            new Color(0x88, 0x88, 0x22));
    }
}
