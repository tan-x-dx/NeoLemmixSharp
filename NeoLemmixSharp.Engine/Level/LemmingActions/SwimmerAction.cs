using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class SwimmerAction : LemmingAction
{
    public static readonly SwimmerAction Instance = new();

    private SwimmerAction()
        : base(
            LemmingActionConstants.SwimmerActionId,
            LemmingActionConstants.SwimmerActionName,
            LemmingActionConstants.SwimmerActionSpriteFileName,
            LemmingActionConstants.SwimmerAnimationFrames,
            LemmingActionConstants.MaxSwimmerPhysicsFrames,
            LemmingActionConstants.PermanentSkillPriority,
            LemmingActionBounds.SwimmerActionBounds)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.AnchorPosition;
        var dx = lemming.FacingDirection.DeltaX;

        lemming.DistanceFallen = 0;
        // Need to set this here for swimmers, as it's not constant.
        // 0 is the fallback value that's correct for *most* situations. Transition will
        // still set TrueDistanceFallen, so we don't need to worry about that one.

        lemmingPosition = orientation.MoveRight(lemmingPosition, dx);

        var dy = FindGroundPixel(lemming, lemmingPosition, in gadgetsNearLemming);

        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, lemmingPosition) ||
            WaterAt(in gadgetsNearLemming, lemming, lemmingPosition))
        {
            // Rise if there is water above the lemming
            var pixelAbove = orientation.MoveUp(lemmingPosition, 1);
            if (dy <= 1 &&
                WaterAt(in gadgetsNearLemming, lemming, pixelAbove) &&
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

                if (!WaterAt(in gadgetsNearLemming, lemming, lemmingPosition))
                {
                    WalkerAction.Instance.TransitionLemmingToAction(lemming, false);

                    return true;
                }

                if (lemming.State.IsClimber &&
                    !WaterAt(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 1)))
                {
                    // Only transition to climber, if the lemming is not under water
                    ClimberAction.Instance.TransitionLemmingToAction(lemming, false);

                    return true;
                }


                lemming.FacingDirection = lemming.FacingDirection.GetOpposite();
                lemmingPosition = orientation.MoveLeft(lemmingPosition, dx); // Move lemming back

                return true;
            }

            if (dy >= 3)
            {
                AscenderAction.Instance.TransitionLemmingToAction(lemming, false);
                lemmingPosition = orientation.MoveUp(lemmingPosition, 2);

                return true;
            }

            if (dy >= 1 || (dy == 0 && !WaterAt(in gadgetsNearLemming, lemming, lemmingPosition)))
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

    [Pure]
    private static bool WaterAt(
        in GadgetEnumerable gadgetEnumerable,
        Lemming lemming,
        Point lemmingPosition)
    {
        foreach (var gadget in gadgetEnumerable)
        {
            if (!gadget.ContainsPoint(lemming.Orientation, lemmingPosition))
                continue;

            var filters = gadget.CurrentState.Filters;

            for (var i = 0; i < filters.Length; i++)
            {
                var filter = filters[i];

                if (filter.MatchesLemming(lemming) &&
                    filter.HitBoxBehaviour == HitBoxInteractionType.Liquid)
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns 0 if the lemming may not dive down. Otherwise return the amount of pixels the lemming dives
    /// </summary>
    private static int LemDive(
        in GadgetEnumerable gadgetsNearLemming,
        Lemming lemming,
        Point lemmingPosition)
    {
        var orientation = lemming.Orientation;
        var result = 1;

        while (result <= 4 && PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveDown(lemmingPosition, result)))
        {
            result++;
            lemming.DistanceFallen++;

            if (WaterAt(in gadgetsNearLemming, lemming, orientation.MoveDown(lemmingPosition, result)))
            {
                lemming.DistanceFallen = 0;
            }
        }

        return result > 4
            ? 0
            : result;
    }

    public override void TransitionLemmingToAction(
        Lemming lemming,
        bool turnAround)
    {
        DoMainTransitionActions(lemming, turnAround);

        DoSwimmerTransitionActions(lemming, turnAround);
    }

    [SkipLocalsInit]
    private static void DoSwimmerTransitionActions(
        Lemming lemming,
        bool turnAround)
    {
        // If possible, float up 4 pixels when starting
        var orientation = lemming.Orientation;
        var checkPosition = orientation.MoveUp(lemming.AnchorPosition, 1);

        var i = 0;

        var gadgetManager = LevelScreen.GadgetManager;
        Span<uint> scratchSpaceSpan = stackalloc uint[gadgetManager.ScratchSpaceSize];
        var gadgetTestRegion = new RectangularRegion(
            orientation.Move(lemming.AnchorPosition, lemming.FacingDirection.DeltaX, 2),
            orientation.MoveDown(lemming.AnchorPosition, 4));
        gadgetManager.GetAllItemsNearRegion(scratchSpaceSpan, gadgetTestRegion, out var gadgetsNearLemming);

        while (i < 4 &&
               WaterAt(in gadgetsNearLemming, lemming, checkPosition) &&
               !PositionIsSolidToLemming(in gadgetsNearLemming, lemming, checkPosition))
        {
            i++;
            checkPosition = orientation.MoveUp(lemming.AnchorPosition, 1 + i);
        }

        lemming.AnchorPosition = orientation.MoveUp(lemming.AnchorPosition, i);
    }
}
