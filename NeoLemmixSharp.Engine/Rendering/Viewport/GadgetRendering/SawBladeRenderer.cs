using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

public sealed class SawBladeRenderer : IViewportObjectRenderer
{
    private readonly SawBladeGadget _sawBladeGadget;

    private Texture2D _sawBladeTexture;

    public SawBladeRenderer(SawBladeGadget sawBladeGadget)
    {
        _sawBladeGadget = sawBladeGadget;
    }

    public void SetSawBladeTexture(Texture2D sawBladeTexture)
    {
        _sawBladeTexture = sawBladeTexture;
    }

    public Rectangle GetSpriteBounds() => _sawBladeGadget.GadgetBounds.ToRectangle();

    public void RenderAtPosition(SpriteBatch spriteBatch,
        Rectangle sourceRectangle,
        int screenX,
        int screenY)
    {
        sourceRectangle.Y += _sawBladeGadget.AnimationFrame * 14;

        var renderDestination = new Rectangle(
            screenX,
            screenY,
            sourceRectangle.Width,
            sourceRectangle.Height);

        spriteBatch.Draw(
            _sawBladeTexture,
            renderDestination,
            sourceRectangle,
            RenderingLayers.AthleteRenderLayer);

        sourceRectangle.X += 14;

        spriteBatch.Draw(
            _sawBladeTexture,
            renderDestination,
            sourceRectangle,
            Color.Gray,
            RenderingLayers.AthleteRenderLayer);
    }

    public void Dispose()
    {
    }
}