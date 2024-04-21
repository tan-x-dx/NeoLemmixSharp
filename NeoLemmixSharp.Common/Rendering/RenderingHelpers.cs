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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Draw(
        this SpriteBatch spriteBatch,
        Texture2D texture,
        Vector2 position,
        Color color,
        float renderLayer)
    {
        spriteBatch.Draw(
            texture,
            position,
            texture.Bounds,
            color,
            0.0f,
            new Vector2(),
            new Vector2(1f, 1f),
            SpriteEffects.None,
            renderLayer);
    }

    public static Vector2 GetSize(this Texture2D texture)
    {
        return new Vector2(texture.Width, texture.Height);
    }
}