using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public static class LemmingActionHelpers
{
    /// <summary>
    /// Find the new ground pixel. 
    /// If result = -4, then at least 4 pixels are air below levelPosition. 
    /// If result = 7, then at least 7 pixels are terrain above levelPosition
    /// </summary>
    [Pure]
    public static int FindGroundPixel(
        Lemming lemming,
        Point levelPosition,
        in GadgetEnumerable gadgetsNearLemming)
    {
        var orientation = lemming.Orientation;

        int result;
        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, levelPosition))
        {
            result = 0;
            while (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(levelPosition, 1 + result)) &&
                   result < EngineConstants.MaxStepUp + 1)
            {
                result++;
            }

            return result;
        }

        result = -1;
        // MoveUp, but step is negative, therefore moves down
        while (!PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(levelPosition, result)) &&
               result > -(EngineConstants.DefaultFallStep + 1))
        {
            result--;
        }

        return result;
    }

    [Pure]
    public static bool PositionIsSolidToLemming(
        in GadgetEnumerable gadgets,
        Lemming lemming,
        Point levelPosition)
    {
        return LevelScreen.TerrainManager.PixelIsSolidToLemming(lemming, levelPosition) ||
               (gadgets.Count > 0 && HasSolidGadgetAtPosition(in gadgets, lemming, levelPosition));
    }

    [Pure]
    public static bool PositionIsIndestructibleToLemming(
        in GadgetEnumerable gadgets,
        Lemming lemming,
        IDestructionMask destructionMask,
        Point levelPosition)
    {
        return LevelScreen.TerrainManager.PixelIsIndestructibleToLemming(lemming, destructionMask, levelPosition) ||
               (gadgets.Count > 0 && HasSteelGadgetAtPosition(in gadgets, lemming, levelPosition));
    }

    [Pure]
    public static bool PositionIsSteelToLemming(
        in GadgetEnumerable gadgets,
        Lemming lemming,
        Point levelPosition)
    {
        return LevelScreen.TerrainManager.PixelIsSteel(levelPosition) ||
               (gadgets.Count > 0 && HasSteelGadgetAtPosition(in gadgets, lemming, levelPosition));
    }

    [Pure]
    private static bool HasSolidGadgetAtPosition(
        in GadgetEnumerable gadgets,
        Lemming lemming,
        Point levelPosition)
    {
        foreach (var gadget in gadgets)
        {
            if (!gadget.ContainsPoint(lemming.Orientation, levelPosition))
                continue;

            var filters = gadget.CurrentState.Filters;

            for (var i = 0; i < filters.Length; i++)
            {
                var filter = filters[i];

                if (filter.MatchesLemming(lemming) &&
                    filter.LemmingSolidityType != LemmingSolidityType.NotSolid)
                    return true;
            }
        }

        return false;
    }

    [Pure]
    private static bool HasSteelGadgetAtPosition(
        in GadgetEnumerable gadgets,
        Lemming lemming,
        Point levelPosition)
    {
        foreach (var gadget in gadgets)
        {
            var currentState = gadget.CurrentState;

            if (!gadget.ContainsPoint(lemming.Orientation, levelPosition))
                continue;

            var filters = currentState.Filters;

            for (var i = 0; i < filters.Length; i++)
            {
                var filter = filters[i];

                if (filter.MatchesLemming(lemming) &&
                    filter.LemmingSolidityType == LemmingSolidityType.Steel)
                    return true;
            }
        }

        return false;
    }

    [Pure]
    public static Point GetUpdraftFallDelta(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        if (gadgetsNearLemming.Count == 0)
            return new Point();

        var lemmingOrientation = lemming.Orientation;
        var lemmingOrientationRotNum = lemmingOrientation.RotNum;

        Span<int> deltas = [0, 0, 0, 0];

        var anchorPosition = lemming.AnchorPosition;
        var footPosition = lemming.FootPosition;

        foreach (var gadget in gadgetsNearLemming)
        {
            if (!gadget.ContainsEitherPoint(lemmingOrientation, anchorPosition, footPosition))
                continue;

            var filters = gadget.CurrentState.Filters;
            LemmingHitBoxFilter? firstMatchingFilter = null;

            for (var i = 0; i < filters.Length; i++)
            {
                var filter = filters[i];

                if (filter.MatchesLemming(lemming) &&
                    filter.HitBoxBehaviour == HitBoxInteractionType.Updraft)
                {
                    firstMatchingFilter = filter;
                    break;
                }
            }

            if (firstMatchingFilter is null || firstMatchingFilter.HitBoxBehaviour != HitBoxInteractionType.Updraft)
                continue;

            var deltaRotNum = gadget.Orientation.RotNum - lemmingOrientationRotNum;
            deltas[deltaRotNum & 3] = 1;
        }

        var dx = deltas[EngineConstants.RightOrientationRotNum] -
                 deltas[EngineConstants.LeftOrientationRotNum];

        var dy = deltas[EngineConstants.UpOrientationRotNum] -
                 deltas[EngineConstants.DownOrientationRotNum];

        return new Point(dx, dy);
    }
}
