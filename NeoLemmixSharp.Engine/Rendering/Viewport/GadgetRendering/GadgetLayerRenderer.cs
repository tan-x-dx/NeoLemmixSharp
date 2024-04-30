﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Level.Gadgets;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

public sealed class GadgetLayerRenderer : IGadgetRenderer
{
    private readonly Texture2D _texture;
    private IControlledAnimationGadget _gadget;

    public GadgetRenderMode RenderMode { get; }
    public int ItemId => _gadget.Id;

    public GadgetLayerRenderer(
        Texture2D texture,
        GadgetRenderMode renderMode)
    {
        _texture = texture;
        RenderMode = renderMode;
    }

    public void SetGadget(IControlledAnimationGadget gadget) => _gadget = gadget;

    public Rectangle GetSpriteBounds() => _gadget.GadgetBounds.ToRectangle();

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int screenX, int screenY)
    {
        var gadgetAnimationController = _gadget.AnimationController;
        var renderDestination = new Rectangle(
            screenX,
            screenY,
            sourceRectangle.Width,
            sourceRectangle.Height);
        gadgetAnimationController.RenderLayers(spriteBatch, _texture, sourceRectangle, renderDestination);
    }

    public void Dispose()
    {
        _gadget = null!;
    }
}