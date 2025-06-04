using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Animations;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public static class GadgetAnimationControllerBuilder
{
    public static AnimationController BuildAnimationController(
        AnimationLayerArchetypeData[] animationLayerData,
        GadgetBounds gadgetBounds,
        Size baseSpriteSize,
        DihedralTransformation dihedralTransformation)
    {
        var animationLayers = BuildAnimationLayers(animationLayerData, baseSpriteSize, dihedralTransformation);

        return new AnimationController(animationLayers, gadgetBounds);
    }

    private static AnimationLayer[] BuildAnimationLayers(
        AnimationLayerArchetypeData[] animationLayerData,
        Size baseSpriteSize,
        DihedralTransformation dihedralTransformation)
    {
        var result = CollectionsHelper.GetArrayForSize<AnimationLayer>(animationLayerData.Length);

        var transformedSpriteSize = dihedralTransformation.Transform(baseSpriteSize);

        for (var i = 0; i < result.Length; i++)
        {
            var animationLayerArchetypeData = animationLayerData[i];
            var transformedNineSliceData = dihedralTransformation.Transform(animationLayerArchetypeData.NineSliceData, baseSpriteSize);
            var nineSliceData = BuildNineSliceData(transformedNineSliceData, transformedSpriteSize);

            result[i] = new AnimationLayer(
                animationLayerArchetypeData.AnimationLayerParameters,
                nineSliceData,
                animationLayerArchetypeData.InitialFrame,
                animationLayerArchetypeData.NextGadgetState);
        }

        return result;
    }

    [SkipLocalsInit]
    private static NineSliceRenderer[] BuildNineSliceData(
        RectangularRegion nineSliceData,
        Size baseSpriteSize)
    {
        var horizontalInterval = nineSliceData.GetHorizontalInterval();
        var numberOfHorizontalIntervals = CalculateNumberOfNineSliceIntervals(horizontalInterval, baseSpriteSize.W);
        Span<Interval> horizontalIntervals = stackalloc Interval[numberOfHorizontalIntervals];
        EvaluateNineSliceIntervals(horizontalIntervals, horizontalInterval, baseSpriteSize.W);

        var verticalInterval = nineSliceData.GetVerticalInterval();
        var numberOfVerticalIntervals = CalculateNumberOfNineSliceIntervals(verticalInterval, baseSpriteSize.H);
        Span<Interval> verticalIntervals = stackalloc Interval[numberOfVerticalIntervals];
        EvaluateNineSliceIntervals(verticalIntervals, verticalInterval, baseSpriteSize.H);

        return GenerateNineSliceRenderers(horizontalIntervals, verticalIntervals);

        static int CalculateNumberOfNineSliceIntervals(
            Interval nineSliceInterval,
            int baseSpriteSize)
        {
            return (nineSliceInterval.Start > 0 ? 1 : 0) +
                   (nineSliceInterval.Start + nineSliceInterval.Length < baseSpriteSize ? 1 : 0) +
                   1;
        }

        static void EvaluateNineSliceIntervals(
            Span<Interval> intervals,
            Interval nineSliceInterval,
            int baseSpriteSize)
        {
            int a;
            int b = 0;
            if (nineSliceInterval.Start > 0)
            {
                a = nineSliceInterval.Start;
                intervals[b++] = new Interval(0, a);
            }
            else
            {
                a = 0;
            }

            if (nineSliceInterval.Start + nineSliceInterval.Length < baseSpriteSize)
            {
                intervals[b++] = new Interval(a, nineSliceInterval.Length);
                intervals[b] = new Interval(nineSliceInterval.Start + nineSliceInterval.Length, baseSpriteSize - (nineSliceInterval.Start + nineSliceInterval.Length));
            }
            else
            {
                intervals[b] = new Interval(a, baseSpriteSize - a);
            }
        }

        static NineSliceRenderer[] GenerateNineSliceRenderers(
            ReadOnlySpan<Interval> horizontalIntervals,
            ReadOnlySpan<Interval> verticalIntervals)
        {
            var s = new Size(horizontalIntervals.Length, verticalIntervals.Length);
            var result = new NineSliceRenderer[s.Area()];

            for (int y = 0; y < verticalIntervals.Length; y++)
            {
                var verticalInterval = verticalIntervals[y];

                for (int x = 0; x < horizontalIntervals.Length; x++)
                {
                    var horizontalInterval = horizontalIntervals[x];

                    var i = s.GetIndexOfPoint(new Point(x, y));
                    var region = new RectangularRegion(horizontalInterval, verticalInterval);
                    result[i] = new NineSliceRenderer(region);
                }
            }

            return result;
        }
    }
}
