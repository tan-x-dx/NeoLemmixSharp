using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public sealed class LemmingActionSprite : IDisposable
{
    public static LemmingActionSprite Empty { get; private set; } = null!;

    public static void Initialise(GraphicsDevice graphicsDevice)
    {
        var texture = new Texture2D(graphicsDevice, 1, 1);
        var data = new[] { 0U };
        texture.SetData(data);

        Empty = new LemmingActionSprite(texture, default, default, []);
    }

    private readonly LemmingActionLayerRenderer[] _renderers;

    public Texture2D Texture { get; }
    public Point AnchorPoint { get; }
    public Size SpriteSize { get; }

    public LemmingActionSprite(
        Texture2D texture,
        Point anchorPoint,
        Size spriteSize,
        LemmingActionLayerRenderer[] renderers)
    {
        Texture = texture;
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
        var orientation = lemming.Data.Orientation;
        var facingDirection = lemming.Data.FacingDirection;

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

    public void Dispose()
    {
        Texture.Dispose();
    }
}