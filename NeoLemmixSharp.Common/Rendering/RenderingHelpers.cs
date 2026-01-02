using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Rendering;

public static class RenderingHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Draw(
        this SpriteBatch spriteBatch,
        Texture2D texture,
        Rectangle destinationRectangle,
        Rectangle sourceRectangle)
    {
        spriteBatch.Draw(
            texture,
            destinationRectangle,
            sourceRectangle,
            Color.White,
            0.0f,
            new Vector2(),
            SpriteEffects.None,
            1.0f);
    }

    public static void FillRect(
        this SpriteBatch spriteBatch,
        Rectangle destinationRectangle,
        Color color)
    {
        spriteBatch.Draw(
            CommonSprites.WhitePixelGradientSprite,
            destinationRectangle,
            CommonSprites.RectangleForWhitePixelAlpha(0xff),
            color);
    }
}