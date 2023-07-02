using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Rendering.Level.Viewport.Lemming;

public sealed class SingleColourLayerActionSprite : ActionSprite
{
    public SingleColourLayerActionSprite(
        Texture2D texture,
        int spriteWidth,
        int spriteHeight,
        int numberOfFrames,
        int numberOfLayers,
        LevelPosition anchorPoint)
        : base(texture, spriteWidth, spriteHeight, numberOfFrames, numberOfLayers, anchorPoint)
    {
    }

    public override void RenderLemming(
        SpriteBatch spriteBatch,
        Engine.Lemming lemming,
        Rectangle sourceRectangle,
        Rectangle destinationRectangle)
    {
        RenderSpriteLayer(spriteBatch, sourceRectangle, destinationRectangle, Color.White, RenderingLayers.LemmingRenderLayer);
    }
}