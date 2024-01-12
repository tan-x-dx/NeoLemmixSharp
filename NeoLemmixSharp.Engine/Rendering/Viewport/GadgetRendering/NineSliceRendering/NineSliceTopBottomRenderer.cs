using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering.NineSliceRendering;

public sealed class NineSliceTopRenderer : NineSliceSubRenderer
{
    private readonly int _sliceLeft;
    private readonly int _sliceRight;
    private readonly int _sliceTop;

    public override bool IsEmpty => _sliceTop == 0;

    public NineSliceTopRenderer(
        IRectangularLevelRegion rectangularLevelRegion,
        int spriteWidth,
        int spriteHeight,
        int sliceLeft,
        int sliceRight,
        int sliceTop)
        : base(rectangularLevelRegion, spriteWidth, spriteHeight)
    {
        _sliceLeft = sliceLeft;
        _sliceRight = sliceRight;
        _sliceTop = sliceTop;
    }

    public override void Render(SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle, int screenX, int screenY, int scaleMultiplier)
    {
        var sourceSubRectangle = new Rectangle(
            _sliceLeft,
            0,
            _sliceRight - _sliceLeft,
            _sliceTop);
        var clipSubRectangle = new Rectangle(
            _sliceLeft,
            0,
            RectangularLevelRegion.W + _sliceRight - _sliceLeft - SpriteWidth,
            _sliceTop);

        Rectangle.Intersect(ref sourceRectangle, ref clipSubRectangle, out var intersection);
        if (intersection.IsEmpty)
            return;

        var numberOfIntervals = (clipSubRectangle.X + clipSubRectangle.Width + sourceSubRectangle.Width - sourceSubRectangle.X - 1) / sourceSubRectangle.Width;

        if (numberOfIntervals == 0)
            return;

        Span<IntervalThing> intervals = stackalloc IntervalThing[numberOfIntervals];
        Populate(intervals, sourceSubRectangle.X, sourceSubRectangle.Width, clipSubRectangle.X, clipSubRectangle.Width);

        var w0 = 0;
        var destX = screenX + (intersection.X - sourceRectangle.X) * scaleMultiplier;
        var destY = screenY + (intersection.Y - sourceRectangle.Y) * scaleMultiplier;

        for (var i = 0; i < intervals.Length; i++)
        {
            ref var interval = ref intervals[i];

            var sourceRectangle0 = new Rectangle(
                _sliceLeft + interval.IntervalStart,
                intersection.Y,
                interval.IntervalWidth,
                intersection.Height);

            var destinationRectangle = new Rectangle(
                destX + (w0 - sourceRectangle.X * i) * scaleMultiplier,
                destY,
                interval.IntervalWidth * scaleMultiplier,
                intersection.Height * scaleMultiplier);

            w0 += interval.IntervalWidth;

            spriteBatch.Draw(
                texture,
                destinationRectangle,
                sourceRectangle0,
                0.9375f);
        }
    }
}

public sealed class NineSliceBottomRenderer : NineSliceSubRenderer
{
    private readonly int _sliceLeft;
    private readonly int _sliceRight;
    private readonly int _sliceBottom;

    public override bool IsEmpty => _sliceBottom == 0;

    public NineSliceBottomRenderer(
        IRectangularLevelRegion rectangularLevelRegion,
        int spriteWidth,
        int spriteHeight,
        int sliceLeft,
        int sliceRight,
        int sliceBottom)
        : base(rectangularLevelRegion, spriteWidth, spriteHeight)
    {
        _sliceLeft = sliceLeft;
        _sliceRight = sliceRight;
        _sliceBottom = sliceBottom;
    }

    public override void Render(SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle, int screenX, int screenY, int scaleMultiplier)
    {
        var sourceSubRectangle = new Rectangle(
            _sliceLeft,
            _sliceBottom,
            _sliceRight - _sliceLeft,
            SpriteHeight - _sliceBottom);
        var clipSubRectangle = new Rectangle(
            _sliceLeft,
            _sliceBottom,
            RectangularLevelRegion.W + _sliceRight - _sliceLeft - SpriteWidth,
            SpriteHeight - _sliceBottom);

        Rectangle.Intersect(ref sourceRectangle, ref clipSubRectangle, out var intersection);
        if (intersection.IsEmpty)
            return;

        var numberOfIntervals = (clipSubRectangle.X + clipSubRectangle.Width + sourceSubRectangle.Width - sourceSubRectangle.X - 1) / sourceSubRectangle.Width;

        if (numberOfIntervals == 0)
            return;

        Span<IntervalThing> intervals = stackalloc IntervalThing[numberOfIntervals];
        Populate(intervals, sourceSubRectangle.X, sourceSubRectangle.Width, clipSubRectangle.X, clipSubRectangle.Width);

        var w0 = 0;
        var destX = screenX + (intersection.X - sourceRectangle.X) * scaleMultiplier;
        var destY = screenY + (intersection.Y - sourceRectangle.Y) * scaleMultiplier;

        for (var i = 0; i < intervals.Length; i++)
        {
            ref var interval = ref intervals[i];

            var sourceRectangle0 = new Rectangle(
                _sliceLeft + interval.IntervalStart,
                intersection.Y,
                interval.IntervalWidth,
                intersection.Height);

            var destinationRectangle = new Rectangle(
                destX + w0 * scaleMultiplier,
                destY,
                interval.IntervalWidth * scaleMultiplier,
                intersection.Height * scaleMultiplier);

            w0 += interval.IntervalWidth;

            spriteBatch.Draw(
                texture,
                destinationRectangle,
                sourceRectangle0,
                0.9375f);
        }
    }
}