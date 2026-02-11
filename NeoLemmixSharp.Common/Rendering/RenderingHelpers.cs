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

    public static void DrawRect(
        this SpriteBatch spriteBatch,
        Rectangle destinationRectangle,
        Color color)
    {
        var top = new Rectangle(destinationRectangle.Left, destinationRectangle.Top, destinationRectangle.Width, 1);
        spriteBatch.Draw(
            CommonSprites.WhitePixelGradientSprite,
            top,
            CommonSprites.RectangleForWhitePixelAlpha(0xff),
            color);

        var left = new Rectangle(destinationRectangle.Left, destinationRectangle.Top, 1, destinationRectangle.Height);
        spriteBatch.Draw(
            CommonSprites.WhitePixelGradientSprite,
            left,
            CommonSprites.RectangleForWhitePixelAlpha(0xff),
            color);

        var bottom = new Rectangle(destinationRectangle.Left, destinationRectangle.Top + destinationRectangle.Height - 1, destinationRectangle.Width, 1);
        spriteBatch.Draw(
            CommonSprites.WhitePixelGradientSprite,
            bottom,
            CommonSprites.RectangleForWhitePixelAlpha(0xff),
            color);

        var right = new Rectangle(destinationRectangle.Left + destinationRectangle.Width - 1, destinationRectangle.Top, 1, destinationRectangle.Height);
        spriteBatch.Draw(
            CommonSprites.WhitePixelGradientSprite,
            right,
            CommonSprites.RectangleForWhitePixelAlpha(0xff),
            color);
    }

    public static void Negate(ref Color color)
    {
        ref uint x = ref Unsafe.As<Color, uint>(ref color);
        x ^= 0x00ffffff;
    }
}
