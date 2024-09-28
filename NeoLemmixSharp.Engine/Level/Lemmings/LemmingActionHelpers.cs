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
    [SkipLocalsInit]
    public static int FindGroundPixel(
        Lemming lemming,
        LevelPosition levelPosition)
    {
        var orientation = lemming.Orientation;

        // Subroutine of other LevelAction methods.
        // Use a dummy scratch space span to prevent data from being overridden.
        // Prevents weird bugs!
        Span<uint> scratchSpace = stackalloc uint[LevelScreen.GadgetManager.ScratchSpaceSize];

        var gadgetTestRegion = new LevelPositionPair(
            orientation.MoveUp(levelPosition, LevelConstants.MaxStepUp + 1),
            orientation.MoveDown(levelPosition, LevelConstants.DefaultFallStep + 1));
        LevelScreen.GadgetManager.GetAllItemsNearRegion(scratchSpace, gadgetTestRegion, out var gadgetsNearRegion);

        int result;
        if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, levelPosition))
        {
            result = 0;
            while (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveUp(levelPosition, 1 + result)) &&
                   result < LevelConstants.MaxStepUp + 1)
            {
                result++;
            }

            return result;
        }

        result = -1;
        // MoveUp, but step is negative, therefore moves down
        while (!PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveUp(levelPosition, result)) &&
               result > -(LevelConstants.DefaultFallStep + 1))
        {
            result--;
        }

        return result;
    }

    [Pure]
    public static bool PositionIsSolidToLemming(
        in GadgetSet gadgets,
        Lemming lemming,
        LevelPosition levelPosition)
    {
        return LevelScreen.TerrainManager.PixelIsSolidToLemming(lemming, levelPosition) ||
               gadgets.Count > 0 && HasSolidGadgetAtPosition(in gadgets, lemming, levelPosition);
    }

    [Pure]
    public static bool PositionIsIndestructibleToLemming(
        in GadgetSet gadgets,
        Lemming lemming,
        IDestructionMask destructionMask,
        LevelPosition levelPosition)
    {
        return LevelScreen.TerrainManager.PixelIsIndestructibleToLemming(lemming, destructionMask, levelPosition) ||
               gadgets.Count > 0 && HasSteelGadgetAtPosition(in gadgets, lemming, levelPosition);
    }

    [Pure]
    public static bool PositionIsSteelToLemming(
        in GadgetSet gadgets,
        Lemming lemming,
        LevelPosition levelPosition)
    {
        return LevelScreen.TerrainManager.PixelIsSteel(levelPosition) ||
               gadgets.Count > 0 && HasSteelGadgetAtPosition(in gadgets, lemming, levelPosition);
    }

    [Pure]
    private static bool HasSolidGadgetAtPosition(
        in GadgetSet gadgets,
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
        in GadgetSet gadgets,
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
}