using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

public sealed class GadgetRenderer : IGadgetRenderer
{
    private readonly GadgetBase _gadget;
    private readonly Texture2D _texture;

    public GadgetRenderMode RenderMode { get; }
    public int RendererId { get; set; }
    public int ItemId { get; }

    public LevelRegion CurrentBounds => _gadget.CurrentAnimationController.CurrentBounds;
    public LevelRegion PreviousBounds => _gadget.CurrentAnimationController.PreviousBounds;

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
