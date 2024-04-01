using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Level.Gadgets;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

public sealed class GadgetRenderer : IGadgetRenderer
{
    private readonly GadgetLayerRenderer[] _animationLayers;

    private GadgetBase _gadget;

    public GadgetRenderMode RenderMode { get; }

    public GadgetRenderer(
        GadgetLayerRenderer[] animationLayers,
        GadgetRenderMode renderMode)
    {
        _animationLayers = animationLayers;
        RenderMode = renderMode;
    }

    public void SetGadget(GadgetBase gadget) => _gadget = gadget;

    public Rectangle GetSpriteBounds() => _gadget.GadgetBounds.ToRectangle();

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int screenX, int screenY)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _gadget = null!;
        Array.Clear(_animationLayers);
    }
}