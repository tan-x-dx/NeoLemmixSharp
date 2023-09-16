﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.Level.Gadgets.Functional.SawBlade;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Gadget;

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

    public void RenderAtPosition(
        SpriteBatch spriteBatch,
        Rectangle sourceRectangle,
        int screenX,
        int screenY,
        int scaleMultiplier)
    {
        sourceRectangle.Y += _sawBladeGadget.AnimationFrame * 14;

        var renderDestination = new Rectangle(
            screenX,
            screenY,
            sourceRectangle.Width * scaleMultiplier,
            sourceRectangle.Height * scaleMultiplier);

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