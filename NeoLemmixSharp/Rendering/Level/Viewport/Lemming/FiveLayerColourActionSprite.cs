﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Rendering.Level.Viewport.Lemming;

public sealed class FiveLayerColourActionSprite : ActionSprite
{
    public FiveLayerColourActionSprite(
        Texture2D texture,
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames,
        int numberOfLayers,
        LevelPosition anchorPoint)
        : base(texture, spriteWidth, spriteHeight, numberOfFrames, numberOfLayers, anchorPoint)
    {
    }

    public override void RenderLemming(SpriteBatch spriteBatch, Engine.Lemming lemming, Rectangle sourceRectangle, Rectangle destinationRectangle)
    {
        RenderSpriteLayer(spriteBatch, sourceRectangle, destinationRectangle, Color.White, RenderingLayers.LemmingRenderLayer);
        sourceRectangle.X += SpriteWidth;
        RenderSpriteLayer(spriteBatch, sourceRectangle, destinationRectangle, lemming.State.HairColor, RenderingLayers.LemmingRenderLayer);
        sourceRectangle.X += SpriteWidth;
        RenderSpriteLayer(spriteBatch, sourceRectangle, destinationRectangle, lemming.State.SkinColor, RenderingLayers.LemmingRenderLayer);
        sourceRectangle.X += SpriteWidth;
        RenderSpriteLayer(spriteBatch, sourceRectangle, destinationRectangle, lemming.State.BodyColor, RenderingLayers.LemmingRenderLayer);
        sourceRectangle.X += SpriteWidth;
        RenderSpriteLayer(spriteBatch, sourceRectangle, destinationRectangle, Color.Magenta, RenderingLayers.LemmingRenderLayer);
    }
}