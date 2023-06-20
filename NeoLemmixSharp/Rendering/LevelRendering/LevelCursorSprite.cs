using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;

namespace NeoLemmixSharp.Rendering.LevelRendering;

public sealed class LevelCursorSprite : ISprite
{
    public const int CursorSizeInPixels = 16;
    public const int HalfCursorSizeInPixels = CursorSizeInPixels / 2;

    private LevelCursor _levelCursor;

    public Texture2D StandardCursorTexture { get; }
    public Texture2D FocusedCursorTexture { get; }

    public LevelCursorSprite(
        Texture2D standardCursorTexture,
        Texture2D focusedCursorTexture)
    {
        StandardCursorTexture = standardCursorTexture;
        FocusedCursorTexture = focusedCursorTexture;
    }

    public void SetLevelCursor(LevelCursor levelCursor)
    {
        _levelCursor = levelCursor;
    }

    public Rectangle GetLocationRectangle() => new(0, 0, CursorSizeInPixels, CursorSizeInPixels);

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
            GetLocationRectangle(),
            Color.White,
            0.0f,
            new Vector2(),
            SpriteEffects.None,
            RenderingLayers.CursorLayer);
    }

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int x, int y, int scaleMultiplier)
    {
        RenderAtPosition(spriteBatch, x, y, scaleMultiplier);
    }

    public void Dispose()
    {
        StandardCursorTexture.Dispose();
        FocusedCursorTexture.Dispose();

        _levelCursor = null;
    }
}