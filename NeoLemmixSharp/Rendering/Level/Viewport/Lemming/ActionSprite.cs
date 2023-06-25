using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Util;
using System;

namespace NeoLemmixSharp.Rendering.Level.Viewport.Lemming;

public abstract class ActionSprite : IDisposable
{
    public int SpriteWidth { get; }
    public int SpriteHeight { get; }
    public int NumberOfFrames { get; }
    public int NumberOfLayers { get; }

    public Texture2D Texture { get; }
    public LevelPosition AnchorPoint { get; }

    protected ActionSprite(
        Texture2D texture,
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames,
        int numberOfLayers,
        LevelPosition anchorPoint)
    {
        Texture = texture;
        SpriteWidth = spriteWidth;
        SpriteHeight = spriteHeight;
        NumberOfFrames = numberOfFrames;
        NumberOfLayers = numberOfLayers;
        AnchorPoint = anchorPoint;
    }

    public abstract void RenderLemming(
        SpriteBatch spriteBatch,
        Engine.Lemming lemming,
        Rectangle sourceRectangle,
        Rectangle destinationRectangle);

    public Rectangle GetSourceRectangleForFrame(int frame)
    {
        return new Rectangle(0, frame * SpriteHeight, SpriteWidth, SpriteHeight);
    }

    public Rectangle GetSourceRectangleForFrame(Rectangle sourceRectangle, int frame)
    {
        sourceRectangle.Y += frame * SpriteHeight;

        return new Rectangle(0, frame * SpriteHeight, SpriteWidth, SpriteHeight);
    }

    public void Dispose()
    {
        Texture.Dispose();
    }

    protected void RenderSpriteLayer(
        SpriteBatch spriteBatch,
        Rectangle sourceRectangle,
        Rectangle destinationRectangle,
        Color layerColor,
        float renderLayer)
    {
        /*
                var renderDestination = new Rectangle(
                    x,
                    y,
                    SpriteWidth * scaleMultiplier,
                    SpriteHeight * scaleMultiplier);
                */
        spriteBatch.Draw(
            Texture,
            destinationRectangle,
            sourceRectangle,
            layerColor,
            0.0f,
            new Vector2(),
            SpriteEffects.None,
            renderLayer);
    }
}