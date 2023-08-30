using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;

public sealed class SingleColorLayerActionSprite : ActionSprite
{
    public SingleColorLayerActionSprite(
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
        Level.Lemmings.Lemming lemming,
        Rectangle sourceRectangle,
        Rectangle destinationRectangle)
    {
        RenderSpriteLayer(spriteBatch, sourceRectangle, destinationRectangle, Color.White, RenderingLayers.LemmingRenderLayer);
    }
}