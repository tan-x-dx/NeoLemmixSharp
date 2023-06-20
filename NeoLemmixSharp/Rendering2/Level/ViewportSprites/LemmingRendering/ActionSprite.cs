﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Rendering.LevelRendering;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Rendering2.Level.ViewportSprites.LemmingRendering;

public abstract class ActionSprite : IDisposable
{
    public abstract int SpriteWidth { get; }
    public abstract int SpriteHeight { get; }
    public abstract int NumberOfFrames { get; }
    public abstract int NumberOfLayers { get; }

    public Texture2D Texture { get; }
    public abstract LevelPosition AnchorPoint { get; }

    protected ActionSprite(
        Texture2D texture)
    {
        Texture = texture;
    }

    public abstract void RenderLemming(
        SpriteBatch spriteBatch,
        Lemming lemming,
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
        Color layerColor)
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
            RenderingLayers.LemmingRenderLayer);
    }
}