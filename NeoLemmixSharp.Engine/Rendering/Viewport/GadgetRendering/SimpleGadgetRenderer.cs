using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

public sealed class SimpleGadgetRenderer : IGadgetRenderer
{
    private readonly Texture2D _texture;
    private readonly LevelRegion _bounds;

    public GadgetRenderMode RenderMode { get; }
    public int RendererId { get; set; }
    public int ItemId { get; }

    public LevelRegion PreviousBounds => _bounds;
    public LevelRegion CurrentBounds => _bounds;

    public SimpleGadgetRenderer(
        Texture2D texture,
        LevelPosition position)
    {
        _texture = texture;
        _bounds = new LevelRegion(position, new LevelSize(texture.Width, texture.Height));
    }

    public Rectangle GetSpriteBounds() => new(_bounds.X, _bounds.Y, _texture.Width, _texture.Height);

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int projectionX, int projectionY)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
