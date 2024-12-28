using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

public sealed class SimpleGadgetRenderer : IGadgetRenderer
{
    private readonly Texture2D _texture;
    private readonly LevelPosition _position;

    public GadgetRenderMode RenderMode { get; }
    public int RendererId { get; set; }
    public int ItemId { get; }

    public LevelPosition TopLeftPixel { get; }
    public LevelPosition BottomRightPixel { get; }
    public LevelPosition PreviousTopLeftPixel { get; }
    public LevelPosition PreviousBottomRightPixel { get; }

    public SimpleGadgetRenderer(
        Texture2D texture,
        LevelPosition position)
    {
        _texture = texture;
        _position = position;

        TopLeftPixel = position;
        BottomRightPixel = position + new LevelPosition(texture.Width, texture.Height);
        PreviousTopLeftPixel = TopLeftPixel;
        PreviousBottomRightPixel = BottomRightPixel;
    }

    public Rectangle GetSpriteBounds() => new(_position.X, _position.Y, _texture.Width, _texture.Height);

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int projectionX, int projectionY)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
