using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

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
            color,
            RenderingLayers.AthleteRenderLayer);
    }

    public void Dispose()
    {

    }
}