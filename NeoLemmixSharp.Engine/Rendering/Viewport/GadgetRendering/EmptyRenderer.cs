using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Level.Gadgets;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

public sealed class EmptyRenderer : IGadgetRenderer
{
    public static readonly EmptyRenderer Instance = new();

    GadgetRenderMode IGadgetRenderer.RenderMode => GadgetRenderMode.NoRender;
    int IViewportObjectRenderer.RendererId { get; set; }
    int IViewportObjectRenderer.ItemId => 0;

    RectangularRegion IRectangularBounds.CurrentBounds => default;

    private EmptyRenderer()
    {
    }

    Rectangle IViewportObjectRenderer.GetSpriteBounds() => default;

    void IViewportObjectRenderer.RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int projectionX, int projectionY)
    {
    }
    void IDisposable.Dispose()
    {
    }
}
