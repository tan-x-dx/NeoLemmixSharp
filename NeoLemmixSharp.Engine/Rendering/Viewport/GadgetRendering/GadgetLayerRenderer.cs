using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

public sealed class GadgetLayerRenderer : IControlledAnimationGadgetRenderer
{
    private readonly Texture2D _texture;
    private HitBoxGadget _gadget;

    public GadgetRenderMode RenderMode { get; }
    public int RendererId { get; set; }
    public int ItemId => _gadget.Id;

    private LevelPosition _currentPosition;
    private LevelPosition _previousPosition;

    public LevelPosition TopLeftPixel => _currentPosition;
    public LevelPosition BottomRightPixel => _currentPosition + new LevelPosition(_texture.Width, _texture.Height);
    public LevelPosition PreviousTopLeftPixel => _previousPosition;
    public LevelPosition PreviousBottomRightPixel => _previousPosition + new LevelPosition(_texture.Width, _texture.Height);

    public GadgetLayerRenderer(
        Texture2D texture,
        GadgetRenderMode renderMode)
    {
        _texture = texture;
        RenderMode = renderMode;
    }

    public void SetGadget(HitBoxGadget gadget) => _gadget = gadget;

    public Rectangle GetSpriteBounds() => new(_currentPosition.X, _currentPosition.Y, _texture.Width, _texture.Height);

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int projectionX, int projectionY)
    {
        var gadgetAnimationController = _gadget.AnimationController;
        var renderDestination = new Rectangle(
            projectionX,
            projectionY,
            sourceRectangle.Width,
            sourceRectangle.Height);
        gadgetAnimationController.RenderLayers(spriteBatch, _texture, sourceRectangle, renderDestination);
    }

    public void Dispose()
    {
        _gadget = null!;
    }
}