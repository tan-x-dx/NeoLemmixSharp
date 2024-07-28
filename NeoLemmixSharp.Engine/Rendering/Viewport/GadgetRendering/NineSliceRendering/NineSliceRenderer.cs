using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering.NineSliceRendering;

public sealed class NineSliceRenderer : INineSliceGadgetRender
{
    private readonly Texture2D _texture;

    public int RendererId { get; set; }
    public int ItemId { get; }
    public GadgetRenderMode RenderMode { get; }

    public LevelPosition TopLeftPixel { get; }
    public LevelPosition BottomRightPixel { get; }
    public LevelPosition PreviousTopLeftPixel { get; }
    public LevelPosition PreviousBottomRightPixel { get; }

    public NineSliceRenderer(Texture2D texture)
    {
        _texture = texture;
    }

    public void SetGadget(IResizeableGadget gadget) => throw new NotSupportedException();

    public Rectangle GetSpriteBounds()
    {
        throw new NotImplementedException();
    }

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int projectionX, int projectionY)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
    }
}