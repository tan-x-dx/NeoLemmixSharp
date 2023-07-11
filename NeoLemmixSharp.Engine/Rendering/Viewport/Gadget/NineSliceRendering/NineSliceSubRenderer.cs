using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util.LevelRegion;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Gadget.NineSliceRendering;

public abstract class NineSliceSubRenderer
{
    protected readonly IRectangularLevelRegion RectangularLevelRegion;
    protected readonly int SpriteWidth;
    protected readonly int SpriteHeight;

    public abstract bool IsEmpty { get; }

    protected NineSliceSubRenderer(
        IRectangularLevelRegion rectangularLevelRegion,
        int spriteWidth,
        int spriteHeight)
    {
        RectangularLevelRegion = rectangularLevelRegion;
        SpriteWidth = spriteWidth;
        SpriteHeight = spriteHeight;
    }

    public abstract void Render(
        SpriteBatch spriteBatch,
        Texture2D texture,
        Rectangle sourceRectangle,
        int screenX,
        int screenY,
        int scaleMultiplier);

    protected readonly struct IntervalThing
    {
        public readonly int IntervalStart;
        public readonly int IntervalWidth;

        public IntervalThing(int intervalStart, int intervalWidth)
        {
            IntervalStart = intervalStart;
            IntervalWidth = intervalWidth;
        }
    }

    protected static void Populate(
        Span<IntervalThing> intervals,
        int maxSize,
        int bigIntervalStart,
        int bigIntervalEnd)
    {
        if (intervals.Length == 1)
        {
            intervals[0] = new IntervalThing(bigIntervalStart, bigIntervalEnd - bigIntervalStart);

            return;
        }

        intervals[0] = new IntervalThing(bigIntervalStart, maxSize - bigIntervalStart);

        var limit = intervals.Length - 1;
        for (var i = 1; i < limit; i++)
        {
            intervals[i] = new IntervalThing(0, maxSize);
        }

        intervals[limit] = new IntervalThing(0, bigIntervalEnd - (limit * maxSize));
    }
}