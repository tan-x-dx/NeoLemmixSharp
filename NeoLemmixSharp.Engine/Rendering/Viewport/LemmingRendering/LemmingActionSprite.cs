using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public sealed class LemmingActionSprite
{
    public static LemmingActionSprite Empty { get; } = new LemmingActionSprite(default, default, []);

    private readonly LemmingActionLayerRenderer[] _renderers;
    public Point AnchorPoint { get; }
    public Size SpriteSize { get; }

    public LemmingActionSprite(
        Point anchorPoint,
        Size spriteSize,
        LemmingActionLayerRenderer[] renderers)
    {
        AnchorPoint = anchorPoint;
        SpriteSize = spriteSize;
        _renderers = renderers;
    }

    public void RenderLemming(
        SpriteBatch spriteBatch,
        Lemming lemming,
        Rectangle sourceRectangle,
        Rectangle destinationRectangle)
    {
        var orientation = lemming.Orientation;
        var facingDirection = lemming.FacingDirection;

        var dht = new DihedralTransformation(orientation, facingDirection);
        var offset = dht.Transform(AnchorPoint, SpriteSize);

        destinationRectangle.X += offset.X;
        destinationRectangle.Y += offset.Y;

        var rotationAngle = orientation.GetRotationAngle();
        var spriteEffects = facingDirection.AsSpriteEffects();

        foreach (var layerRenderer in _renderers)
        {
            layerRenderer.RenderLayer(
                spriteBatch,
                lemming.State,
                sourceRectangle,
                destinationRectangle,
                rotationAngle,
                spriteEffects);
        }
    }
}
