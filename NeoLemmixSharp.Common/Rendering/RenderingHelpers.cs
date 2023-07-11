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
        Rectangle sourceRectangle,
        float renderLayer)
    {
        spriteBatch.Draw(
            texture,
            destinationRectangle,
            sourceRectangle,
            Color.White,
            0.0f,
            new Vector2(),
            SpriteEffects.None,
            renderLayer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Draw(
        this SpriteBatch spriteBatch,
        Texture2D texture,
        Rectangle destinationRectangle,
        Rectangle sourceRectangle,
        Color color,
        float renderLayer)
    {
        spriteBatch.Draw(
            texture,
            destinationRectangle,
            sourceRectangle,
            color,
            0.0f,
            new Vector2(),
            SpriteEffects.None,
            renderLayer);
    }
}