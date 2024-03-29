﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.Level;

namespace NeoLemmixSharp.Engine.Rendering.Viewport;

public sealed class LevelCursorSprite : IViewportObjectRenderer
{
    private readonly LevelCursor _levelCursor;
    private readonly Texture2D _cursorTexture;

    public LevelCursorSprite(
        LevelCursor levelCursor,
        Texture2D cursorTexture)
    {
        _levelCursor = levelCursor;
        _cursorTexture = cursorTexture;
    }

    public Rectangle GetSpriteBounds() => new(0, 0, LevelConstants.CursorSizeInPixels, LevelConstants.CursorSizeInPixels);

    public void RenderAtPosition(SpriteBatch spriteBatch, int x, int y, int scaleMultiplier)
    {
        var d = LevelConstants.HalfCursorSizeInPixels * scaleMultiplier;
        var s = LevelConstants.CursorSizeInPixels * scaleMultiplier;

        var destRectangle = new Rectangle(
            x - d,
            y - d,
            s,
            s);

        var sourceRect = GetSpriteBounds();
        var sourceY = _levelCursor.NumberOfLemmingsUnderCursor > 0
            ? LevelConstants.CursorSizeInPixels
            : 0;

        sourceRect.Y = sourceY;

        spriteBatch.Draw(
            _cursorTexture,
            destRectangle,
            sourceRect,
            _levelCursor.Color1,
            RenderingLayers.CursorLayer);

        sourceRect.X += LevelConstants.CursorSizeInPixels;

        spriteBatch.Draw(
            _cursorTexture,
            destRectangle,
            sourceRect,
            _levelCursor.Color2,
            RenderingLayers.CursorLayer);

        sourceRect.X += LevelConstants.CursorSizeInPixels;

        spriteBatch.Draw(
            _cursorTexture,
            destRectangle,
            sourceRect,
            _levelCursor.Color3,
            RenderingLayers.CursorLayer);
    }

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int screenX, int screenY)
    {
        RenderAtPosition(spriteBatch, screenX, screenY, 1);
    }

    public void Dispose()
    {
    }
}