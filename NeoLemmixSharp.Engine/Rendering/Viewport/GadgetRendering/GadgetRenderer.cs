using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Level.Gadget;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

public sealed class GadgetRenderer : IGadgetRenderer
{
    private readonly GadgetBase _gadget;
    private readonly Texture2D _texture;

    public GadgetRenderMode RenderMode { get; }
    public int RendererId { get; set; }
    public int ItemId { get; }

    public RectangularRegion CurrentBounds => new(); //_gadget.CurrentAnimationController.CurrentBounds;

    public GadgetRenderer(
        GadgetBase gadget,
        Texture2D texture,
        GadgetRenderMode renderMode)
    {
        _gadget = gadget;
        _texture = texture;

        RenderMode = renderMode;
    }

    public Rectangle GetSpriteBounds() => CurrentBounds.ToRectangle();

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int projectionX, int projectionY)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
