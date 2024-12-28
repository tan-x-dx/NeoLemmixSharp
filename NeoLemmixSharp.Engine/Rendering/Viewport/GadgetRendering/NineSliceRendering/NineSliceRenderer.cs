using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering.NineSliceRendering;

public sealed class NineSliceRenderer : INineSliceGadgetRender
{
    private readonly Texture2D _texture;
    private HitBoxGadget _gadget;

    private NineSliceDataBuffer _nineSliceDataBuffer;

    private LevelPosition _currentPosition;
    private LevelPosition _previousPosition;

    private int _currentWidth;
    private int _currentHeight;
    private int _previousWidth;
    private int _previousHeight;

    public GadgetRenderMode RenderMode { get; }
    public int RendererId { get; set; }
    public int ItemId => _gadget.Id;

    public LevelPosition TopLeftPixel => _currentPosition;
    public LevelPosition BottomRightPixel => _currentPosition + new LevelPosition(_currentWidth, _currentHeight);
    public LevelPosition PreviousTopLeftPixel => _previousPosition;
    public LevelPosition PreviousBottomRightPixel => _previousPosition + new LevelPosition(_previousWidth, _previousHeight);

    public NineSliceRenderer(
        Texture2D texture,
        GadgetRenderMode renderMode)
    {
        _texture = texture;
        RenderMode = renderMode;

        RecomputeNineSliceProportions();
    }

    public void RecomputeNineSliceProportions()
    {
    }

    public void SetGadget(HitBoxGadget gadget)
    {
        _gadget = gadget;
    }

    public Rectangle GetSpriteBounds() => new(_currentPosition.X, _currentPosition.Y, _currentWidth, _currentHeight);

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int projectionX, int projectionY)
    {
        var whitePixel = CommonSprites.WhitePixelGradientSprite;
        var renderDestination = new Rectangle(
            projectionX,
            projectionY,
            sourceRectangle.Width,
            sourceRectangle.Height);

        spriteBatch.Draw(
            whitePixel,
            renderDestination,
            CommonSprites.RectangleForWhitePixelAlpha(0xff),
            Color.Yellow);
    }

    public void Dispose()
    {
        _gadget = null!;
    }

    [InlineArray(9)]
    private struct NineSliceDataBuffer
    {
        private NineSliceData _firstElement;
    }

    private readonly struct NineSliceData
    {
        public readonly int HorizontalDrawCount;
        public readonly int VerticalDrawCount;


    }
}