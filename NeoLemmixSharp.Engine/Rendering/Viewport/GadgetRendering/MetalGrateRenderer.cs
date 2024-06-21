using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

public sealed class MetalGrateRenderer : IViewportObjectRenderer
{
    private readonly MetalGrateGadget _metalGrateGadget;

    private readonly Texture2D _whitePixelTexture = CommonSprites.WhitePixelGradientSprite;

    public MetalGrateRenderer(MetalGrateGadget metalGrateGadget)
    {
        _metalGrateGadget = metalGrateGadget;
    }

    public int RendererId { get; set; }
    public int ItemId => _metalGrateGadget.Id;
    public Rectangle GetSpriteBounds() => _metalGrateGadget.GadgetBounds.ToRectangle();

    public void RenderAtPosition(SpriteBatch spriteBatch,
        Rectangle sourceRectangle,
        int screenX,
        int screenY)
    {
        Color color;
        if (_metalGrateGadget.CurrentState == MetalGrateGadget.MetalGrateState.On)
        {
            color = Color.Green;
        }
        else if (_metalGrateGadget.CurrentState == MetalGrateGadget.MetalGrateState.Off)
        {
            color = Color.Red;
        }
        else
        {
            color = Color.Magenta;
        }

        var renderDestination = new Rectangle(
            screenX,
            screenY,
            sourceRectangle.Width,
            sourceRectangle.Height);

        spriteBatch.Draw(
            _whitePixelTexture,
            renderDestination,
            sourceRectangle,
            color);
    }

    public void Dispose()
    {

    }

    public LevelPosition TopLeftPixel => _metalGrateGadget.TopLeftPixel;
    public LevelPosition BottomRightPixel => _metalGrateGadget.BottomRightPixel;
    public LevelPosition PreviousTopLeftPixel => _metalGrateGadget.PreviousTopLeftPixel;
    public LevelPosition PreviousBottomRightPixel => _metalGrateGadget.PreviousBottomRightPixel;
}