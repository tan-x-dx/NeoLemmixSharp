using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

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
        LevelPosition levelPosition,
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
        LevelPosition levelPosition)
    {
        return LevelScreen.TerrainManager.PixelIsSolidToLemming(lemming, levelPosition) ||
               (gadgets.Count > 0 && HasSolidGadgetAtPosition(in gadgets, lemming, levelPosition));
    }

    [Pure]
    public static bool PositionIsIndestructibleToLemming(
        in GadgetEnumerable gadgets,
        Lemming lemming,
        IDestructionMask destructionMask,
        LevelPosition levelPosition)
    {
        return LevelScreen.TerrainManager.PixelIsIndestructibleToLemming(lemming, destructionMask, levelPosition) ||
               (gadgets.Count > 0 && HasSteelGadgetAtPosition(in gadgets, lemming, levelPosition));
    }

    [Pure]
    public static bool PositionIsSteelToLemming(
        in GadgetEnumerable gadgets,
        Lemming lemming,
        LevelPosition levelPosition)
    {
        return LevelScreen.TerrainManager.PixelIsSteel(levelPosition) ||
               (gadgets.Count > 0 && HasSteelGadgetAtPosition(in gadgets, lemming, levelPosition));
    }

    [Pure]
    private static bool HasSolidGadgetAtPosition(
        in GadgetEnumerable gadgets,
        Lemming lemming,
        LevelPosition levelPosition)
    {
        foreach (var gadget in gadgets)
        {
            if (gadget.IsSolidToLemmingAtPosition(lemming, levelPosition))
                return true;
        }

        return false;
    }

    [Pure]
    private static bool HasSteelGadgetAtPosition(
        in GadgetEnumerable gadgets,
        Lemming lemming,
        LevelPosition levelPosition)
    {
        foreach (var gadget in gadgets)
        {
            if (gadget.IsSteelToLemmingAtPosition(lemming, levelPosition))
                return true;
        }

        return false;
    }

    [Pure]
    [SkipLocalsInit]
    public static LevelPosition GetUpdraftFallDelta(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        if (gadgetsNearLemming.Count == 0)
            return new LevelPosition();

        var lemmingOrientationRotNum = lemming.Orientation.RotNum;

        var draftDirectionDeltas = new UpdraftBuffer();

        foreach (var gadget in gadgetsNearLemming)
        {/*
            if (gadget.GadgetBehaviour != UpdraftGadgetBehaviour.Instance || !gadget.MatchesLemming(lemming))
                continue;*/

            var deltaRotNum = gadget.Orientation.RotNum - lemmingOrientationRotNum;

            draftDirectionDeltas[deltaRotNum & 3] = 1;
        }

        var dx = draftDirectionDeltas[EngineConstants.RightOrientationRotNum] -
                 draftDirectionDeltas[EngineConstants.LeftOrientationRotNum];

        var dy = draftDirectionDeltas[EngineConstants.UpOrientationRotNum] -
                 draftDirectionDeltas[EngineConstants.DownOrientationRotNum];

        return new LevelPosition(dx, dy);
    }

    [InlineArray(4)]
    private struct UpdraftBuffer
    {
        private int _firstElement;
    }
}