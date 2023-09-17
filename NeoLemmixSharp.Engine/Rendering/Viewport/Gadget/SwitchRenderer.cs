using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Gadget;

public sealed class SwitchRenderer : IViewportObjectRenderer
{
    private readonly SwitchGadget _switchGadget;

    private Texture2D _switchTexture;

    public SwitchRenderer(SwitchGadget switchGadget)
    {
        _switchGadget = switchGadget;
    }

    public void SetSwitchTexture(Texture2D switchTexture)
    {
        _switchTexture = switchTexture;
    }

    public Rectangle GetSpriteBounds() => _switchGadget.GadgetBounds.ToRectangle();

    public void RenderAtPosition(
        SpriteBatch spriteBatch,
        Rectangle sourceRectangle,
        int screenX,
        int screenY,
        int scaleMultiplier)
    {
        sourceRectangle.Y += _switchGadget.AnimationFrame * 13;

        var renderDestination = new Rectangle(
            screenX,
            screenY,
            sourceRectangle.Width * scaleMultiplier,
            sourceRectangle.Height * scaleMultiplier);

        spriteBatch.Draw(
            _switchTexture,
            renderDestination,
            sourceRectangle,
            RenderingLayers.AthleteRenderLayer);

        sourceRectangle.X += 19;

        spriteBatch.Draw(
            _switchTexture,
            renderDestination,
            sourceRectangle,
            Color.Green,
            RenderingLayers.AthleteRenderLayer);
    }

    public void Dispose()
    {

    }
}