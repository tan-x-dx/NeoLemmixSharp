using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering.NineSliceRendering;

public sealed class NineSliceRenderer : INineSliceGadgetRender
{
    private readonly Texture2D _texture;
    private IResizeableGadget _gadget;

    public GadgetRenderMode RenderMode { get; }
    public int RendererId { get; set; }
    public int ItemId => _gadget.Id;
    
    public LevelPosition TopLeftPixel => _gadget.TopLeftPixel;
    public LevelPosition BottomRightPixel => _gadget.BottomRightPixel;
    public LevelPosition PreviousTopLeftPixel => _gadget.PreviousTopLeftPixel;
    public LevelPosition PreviousBottomRightPixel => _gadget.PreviousBottomRightPixel;

    public NineSliceRenderer(
        Texture2D texture,
        GadgetRenderMode renderMode)
    {
        _texture = texture;
        RenderMode = renderMode;
    }

    public void SetGadget(IResizeableGadget gadget) => _gadget = gadget;

    public Rectangle GetSpriteBounds() => _gadget.GadgetBounds.ToRectangle();

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int projectionX, int projectionY)
    {
        var whitePixel = CommonSprites.WhitePixelGradientSprite;
        var renderDestination = new Rectangle(
            projectionX,
            projectionY,
            sourceRectangle.Width,
            sourceRectangle.Height);

        spriteBatch.Draw(
            whitePixel,
            renderDestination,
            CommonSprites.RectangleForWhitePixelAlpha(0xff),
            Color.Yellow);
    }

    public void Dispose()
    {
        _gadget = null!;
    }
}