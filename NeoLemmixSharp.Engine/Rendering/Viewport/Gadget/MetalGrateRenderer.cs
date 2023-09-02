using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Functional;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Gadget;

public sealed class MetalGrateRenderer : IViewportObjectRenderer
{
    private readonly MetalGrateGadget _metalGrateGadget;

    private Texture2D _whitePixelTexture;

    public MetalGrateRenderer(MetalGrateGadget metalGrateGadget)
    {
        _metalGrateGadget = metalGrateGadget;
    }

    public void SetWhitePixelTexture(Texture2D whitePixelTexture)
    {
        _whitePixelTexture = whitePixelTexture;
    }

    public Rectangle GetSpriteBounds() => _metalGrateGadget.GadgetBounds.ToRectangle();

    public void RenderAtPosition(
        SpriteBatch spriteBatch,
        Rectangle sourceRectangle,
        int screenX,
        int screenY,
        int scaleMultiplier)
    {
        Color color;
        if (_metalGrateGadget.Type == GadgetType.MetalGrateOn)
        {
            color = Color.Green;
        }
        else if (_metalGrateGadget.Type == GadgetType.MetalGrateOff)
        {
            color = Color.Red;
        }
        else
        {
            color = Color.Magenta;
        }

        if (!_metalGrateGadget.IsActive)
            return;

        var renderDestination = new Rectangle(
            screenX,
            screenY,
            sourceRectangle.Width * scaleMultiplier,
            sourceRectangle.Height * scaleMultiplier);

        spriteBatch.Draw(
            _whitePixelTexture,
            renderDestination,
            sourceRectangle,
            color,
            RenderingLayers.AthleteRenderLayer);
    }

    public void Dispose()
    {

    }
}