using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.Level;

namespace NeoLemmixSharp.Engine.Rendering.Viewport;

public sealed class LevelCursorSprite : IViewportObjectRenderer
{
    public const int CursorSizeInPixels = 16;
    public const int HalfCursorSizeInPixels = CursorSizeInPixels / 2;

    private readonly LevelCursor _levelCursor;
    private readonly Texture2D _standardCursorTexture;
    private readonly Texture2D _focusedCursorTexture;

    public LevelCursorSprite(
        LevelCursor levelCursor,
        Texture2D standardCursorTexture,
        Texture2D focusedCursorTexture)
    {
        _levelCursor = levelCursor;
        _standardCursorTexture = standardCursorTexture;
        _focusedCursorTexture = focusedCursorTexture;
    }

    public Rectangle GetSpriteBounds() => new(0, 0, CursorSizeInPixels, CursorSizeInPixels);

    public void RenderAtPosition(SpriteBatch spriteBatch, int x, int y, int scaleMultiplier)
    {
        var texture = _levelCursor.NumberOfLemmingsUnderCursor > 0
            ? _focusedCursorTexture
            : _standardCursorTexture;

        var d = HalfCursorSizeInPixels * scaleMultiplier;
        var s = CursorSizeInPixels * scaleMultiplier;

        var destRectangle = new Rectangle(
            x - d,
            y - d,
            s,
            s);

        spriteBatch.Draw(
            texture,
            destRectangle,
            GetSpriteBounds(),
            RenderingLayers.CursorLayer);
    }

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int screenX, int screenY, int scaleMultiplier)
    {
        RenderAtPosition(spriteBatch, screenX, screenY, scaleMultiplier);
    }

    public void Dispose()
    {
    }
}