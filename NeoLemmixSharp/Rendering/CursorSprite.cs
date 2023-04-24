using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;

namespace NeoLemmixSharp.Rendering;

public sealed class CursorSprite : ISprite
{
    private const int CursorSizeInPixels = 16;
    private const int HalfCursorSizeInPixels = CursorSizeInPixels / 2;

    public Texture2D StandardCursorTexture { get; }
    public Texture2D FocusedCursorTexture { get; }

    public CursorSprite(Texture2D standardCursorTexture, Texture2D focusedCursorTexture)
    {
        StandardCursorTexture = standardCursorTexture;
        FocusedCursorTexture = focusedCursorTexture;
    }

    public Rectangle GetLocationRectangle() => new(0, 0, CursorSizeInPixels, CursorSizeInPixels);

    public void RenderAtPosition(SpriteBatch spriteBatch, int x, int y, int scaleMultiplier)
    {
        var texture = LevelScreen.CurrentLevel.LemmingsUnderCursor
            ? FocusedCursorTexture
            : StandardCursorTexture;

        var destRectangle = new Rectangle(
            (x - HalfCursorSizeInPixels) * scaleMultiplier,
            (y - HalfCursorSizeInPixels) * scaleMultiplier,
            CursorSizeInPixels * scaleMultiplier,
            CursorSizeInPixels * scaleMultiplier);

        spriteBatch.Draw(texture, destRectangle, GetLocationRectangle(), Color.White);
    }

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int x, int y, int scaleMultiplier)
    {
        RenderAtPosition(spriteBatch, x, y, scaleMultiplier);
    }

    public void Dispose()
    {
        StandardCursorTexture.Dispose();
        FocusedCursorTexture.Dispose();
    }
}