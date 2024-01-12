using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public sealed class SingleColorLayerActionSprite : ActionSprite
{
    public SingleColorLayerActionSprite(
        Texture2D texture,
        int spriteWidth,
        int spriteHeight,
        LevelPosition anchorPoint)
        : base(texture, spriteWidth, spriteHeight, anchorPoint)
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