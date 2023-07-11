using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util.LevelRegion;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Gadget.NineSliceRendering;

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
      /*  var subRectangle = SubRectangle;
        Rectangle.Intersect(ref sourceRectangle, ref subRectangle, out var intersection);
        if (intersection.IsEmpty)
            return;

        var numberOfIntervals = (intersection.Width + subRectangle.Width - 1) / subRectangle.Width;

        Span<IntervalThing> intervals = stackalloc IntervalThing[numberOfIntervals];
        Populate(intervals, SubRectangle.Width, sourceRectangle.X, sourceRectangle.Right);

        for (var i = 0; i < intervals.Length; i++)
        {
            var interval = intervals[i];

            var sourceRectangle0 = new Rectangle(
                interval.IntervalStart,
                intersection.Y,
                interval.IntervalWidth,
                intersection.Height);

            var destinationRectangle = new Rectangle(
                screenX + (intersection.X * scaleMultiplier),
                screenY + (intersection.Y * scaleMultiplier),
                sourceRectangle0.Width * scaleMultiplier,
                sourceRectangle0.Height * scaleMultiplier);

            spriteBatch.Draw(
                texture,
                destinationRectangle,
                sourceRectangle0,
                0.9375f);
        }*/
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

    public override void Render(SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle, int screenX, int screenY,
        int scaleMultiplier)
    {
    }
}