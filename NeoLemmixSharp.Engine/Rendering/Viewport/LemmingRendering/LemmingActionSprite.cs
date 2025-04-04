using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public sealed class LemmingActionSprite : IDisposable
{
    public static LemmingActionSprite Empty { get; private set; } = null!;

    public static void Initialise(GraphicsDevice graphicsDevice)
    {
        var texture = new Texture2D(graphicsDevice, 1, 1);
        var data = new[] { 0U };
        texture.SetData(data);

        Empty = new LemmingActionSprite(texture, new LevelPosition(), new LevelSize(), Array.Empty<LemmingActionLayerRenderer>());
    }

    private readonly LemmingActionLayerRenderer[] _renderers;

    public Texture2D Texture { get; }
    public LevelPosition AnchorPoint { get; }
    public LevelSize SpriteSize { get; }

    public LemmingActionSprite(
        Texture2D texture,
        LevelPosition anchorPoint,
        LevelSize spriteSize,
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
        var dht = new DihedralTransformation(lemming.Orientation, lemming.FacingDirection);
        var offset = dht.Transform(AnchorPoint, SpriteSize);

        destinationRectangle.X += offset.X;
        destinationRectangle.Y += offset.Y;

        var rotationAngle = lemming.Orientation.GetRotationAngle();
        var spriteEffects = lemming.FacingDirection.AsSpriteEffects();

        foreach (var layerRenderer in _renderers)
        {
            layerRenderer.RenderLayer(
                spriteBatch,
                lemming,
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