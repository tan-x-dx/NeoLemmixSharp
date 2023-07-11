using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.Engine;

namespace NeoLemmixSharp.Engine.Rendering.Viewport;

public sealed class LevelCursorSprite : IViewportObjectRenderer
{
    public const int CursorSizeInPixels = 16;
    public const int HalfCursorSizeInPixels = CursorSizeInPixels / 2;

    private readonly LevelCursor _levelCursor;

    public Texture2D StandardCursorTexture { get; }
    public Texture2D FocusedCursorTexture { get; }

    public LevelCursorSprite(
        LevelCursor levelCursor,
        Texture2D standardCursorTexture,
        Texture2D focusedCursorTexture)
    {
        _levelCursor = levelCursor;
        StandardCursorTexture = standardCursorTexture;
        FocusedCursorTexture = focusedCursorTexture;
    }

    public Rectangle GetSpriteBounds() => new(0, 0, CursorSizeInPixels, CursorSizeInPixels);

    public void RenderAtPosition(SpriteBatch spriteBatch, int x, int y, int scaleMultiplier)
    {
        var texture = _levelCursor.LemmingsUnderCursor
            ? FocusedCursorTexture
            : StandardCursorTexture;

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
        StandardCursorTexture.Dispose();
        FocusedCursorTexture.Dispose();
    }
}