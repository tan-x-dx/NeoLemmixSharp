using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;

public abstract class ActionSprite : IDisposable
{
    public int SpriteWidth { get; }
    public int SpriteHeight { get; }

    public Texture2D Texture { get; }
    public LevelPosition AnchorPoint { get; }

    protected ActionSprite(
        Texture2D texture,
        int spriteWidth,
        int spriteHeight,
        LevelPosition anchorPoint)
    {
        Texture = texture;
        SpriteWidth = spriteWidth;
        SpriteHeight = spriteHeight;
        AnchorPoint = anchorPoint;
    }

    public abstract void RenderLemming(
        SpriteBatch spriteBatch,
        Level.Lemmings.Lemming lemming,
        Rectangle sourceRectangle,
        Rectangle destinationRectangle);

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
        spriteBatch.Draw(
            Texture,
            destinationRectangle,
            sourceRectangle,
            layerColor,
            renderLayer);
    }
}