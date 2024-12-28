using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class SwimmerAction : LemmingAction
{
    public static readonly SwimmerAction Instance = new();

    private SwimmerAction()
        : base(
            EngineConstants.SwimmerActionId,
            EngineConstants.SwimmerActionName,
            EngineConstants.SwimmerActionSpriteFileName,
            EngineConstants.SwimmerAnimationFrames,
            EngineConstants.MaxSwimmerPhysicsFrames,
            EngineConstants.PermanentSkillPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;
        var dx = lemming.FacingDirection.DeltaX;

        lemming.DistanceFallen = 0;
        // Need to set this here for swimmers, as it's not constant.
        // 0 is the fallback value that's correct for *most* situations. Transition will
        // still set TrueDistanceFallen, so we don't need to worry about that one.

        lemmingPosition = orientation.MoveRight(lemmingPosition, dx);

        var dy = FindGroundPixel(lemming, lemmingPosition, in gadgetsNearLemming);

        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, lemmingPosition) ||
            WaterAt(in gadgetsNearLemming, lemmingPosition))
        {
            // Rise if there is water above the lemming
            var pixelAbove = orientation.MoveUp(lemmingPosition, 1);
            if (dy <= 1 &&
                WaterAt(in gadgetsNearLemming, pixelAbove) &&
                !PositionIsSolidToLemming(in gadgetsNearLemming, lemming, pixelAbove))
            {
                lemmingPosition = pixelAbove;

                return true;
            }

            if (dy > 6)
            {
                var diveDistance = LemDive(gadgetsNearLemming, lemming, lemmingPosition);

                if (diveDistance <= 0)
                    return true;

                lemmingPosition = orientation.MoveDown(lemmingPosition, diveDistance); // Dive below the terrain

                if (!WaterAt(in gadgetsNearLemming, lemmingPosition))
                {
                    WalkerAction.Instance.TransitionLemmingToAction(lemming, false);

                    return true;
                }

                if (lemming.State.IsClimber &&
                    !WaterAt(in gadgetsNearLemming, orientation.MoveUp(lemmingPosition, 1)))
                {
                    // Only transition to climber, if the lemming is not under water
                    ClimberAction.Instance.TransitionLemmingToAction(lemming, false);

                    return true;
                }


                lemming.SetFacingDirection(lemming.FacingDirection.GetOpposite());
                lemmingPosition = orientation.MoveLeft(lemmingPosition, dx); // Move lemming back

                return true;
            }

            if (dy >= 3)
            {
                AscenderAction.Instance.TransitionLemmingToAction(lemming, false);
                lemmingPosition = orientation.MoveUp(lemmingPosition, 2);

                return true;
            }

            if (dy >= 1 || (dy == 0 && !WaterAt(in gadgetsNearLemming, lemmingPosition)))
            {
                // see http://www.lemmingsforums.net/index.php?topic=3380.0
                // And the swimmer should not yet stop if the water and terrain overlaps

                WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
                lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
            }

            return true;
        }

        // if no water or terrain on current position
        if (dy < -1)
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        // if dy == 0 or == -1
        lemmingPosition = orientation.MoveUp(lemmingPosition, dy);
        WalkerAction.Instance.TransitionLemmingToAction(lemming, false);

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -7;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 4;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 5;

    [Pure]
    private static bool WaterAt(in GadgetEnumerable gadgetEnumerable, LevelPosition lemmingPosition)
    {
        foreach (var gadget in gadgetEnumerable)
        {
            if (/*gadget.GadgetBehaviour == WaterGadgetBehaviour.Instance &&*/
                gadget.MatchesPosition(lemmingPosition))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Returns 0 if the lemming may not dive down. Otherwise return the amount of pixels the lemming dives
    /// </summary>
    private static int LemDive(
        in GadgetEnumerable gadgetsNearLemming,
        Lemming lemming,
        LevelPosition lemmingPosition)
    {
        var result = 1;

        while (result <= 4 && PositionIsSolidToLemming(in gadgetsNearLemming, lemming, lemming.Orientation.MoveDown(lemmingPosition, result)))
        {
            result++;
            lemming.DistanceFallen++;

            if (WaterAt(
                    in gadgetsNearLemming,
                    lemming.Orientation.MoveDown(lemmingPosition, result)))
            {
                lemming.DistanceFallen = 0;
            }
        }

        return result > 4
            ? 0
            : result;
    }

    [SkipLocalsInit]
    public override void TransitionLemmingToAction(
        Lemming lemming,
        bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        // If possible, float up 4 pixels when starting
        var orientation = lemming.Orientation;
        var checkPosition = orientation.MoveUp(lemming.LevelPosition, 1);

        var i = 0;

        var gadgetManager = LevelScreen.GadgetManager;
        Span<uint> scratchSpaceSpan = stackalloc uint[gadgetManager.ScratchSpaceSize];
        var gadgetTestRegion = new LevelRegion(
            orientation.Move(lemming.LevelPosition, lemming.FacingDirection.DeltaX, 2),
            orientation.MoveDown(lemming.LevelPosition, 4));
        gadgetManager.GetAllItemsNearRegion(scratchSpaceSpan, gadgetTestRegion, out var gadgetsNearLemming);

        while (i < 4 &&
               WaterAt(in gadgetsNearLemming, checkPosition) &&
               !PositionIsSolidToLemming(in gadgetsNearLemming, lemming, checkPosition))
        {
            i++;
            checkPosition = orientation.MoveUp(lemming.LevelPosition, 1 + i);
        }

        lemming.LevelPosition = orientation.MoveUp(lemming.LevelPosition, i);
    }
}