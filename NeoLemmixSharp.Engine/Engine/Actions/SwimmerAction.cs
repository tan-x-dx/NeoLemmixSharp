using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Gadgets.Collections;
using NeoLemmixSharp.Engine.Engine.Lemmings;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class SwimmerAction : LemmingAction
{
    public static SwimmerAction Instance { get; } = new();

    private SwimmerAction()
    {
    }

    public override int Id => GameConstants.SwimmerActionId;
    public override string LemmingActionName => "swimmer";
    public override int NumberOfAnimationFrames => GameConstants.SwimmerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => GameConstants.PermanentSkillPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;
        var dx = lemming.FacingDirection.DeltaX;

        lemming.DistanceFallen = 0;
        // Need to set this here for swimmers, as it's not constant.
        // 0 is the fallback value that's correct for *most* situations. Transition will
        // still set TrueDistanceFallen so we don't need to worry about that one.

        lemmingPosition = orientation.MoveRight(lemmingPosition, dx);
        lemming.LevelPosition = lemmingPosition;

        var dy = FindGroundPixel(lemming, lemmingPosition);

        if (Terrain.PixelIsSolidToLemming(lemming, lemmingPosition) ||
            WaterAt(lemming, lemmingPosition))
        {
            // Rise if there is water above the lemming
            var pixelAbove = orientation.MoveUp(lemmingPosition, 1);
            if (dy >= -1 &&
                WaterAt(lemming, pixelAbove) &&
                !Terrain.PixelIsSolidToLemming(lemming, pixelAbove))
            {
                lemmingPosition = pixelAbove;
                lemming.LevelPosition = lemmingPosition;

                return true;
            }

            if (dy < -6)
            {
                var diveDistance = LemDive(lemming, lemmingPosition);

                if (diveDistance <= 0)
                    return true;

                lemmingPosition = orientation.MoveDown(lemmingPosition, diveDistance); // Dive below the terrain
                lemming.LevelPosition = lemmingPosition;

                if (!WaterAt(lemming, lemmingPosition))
                {
                    WalkerAction.Instance.TransitionLemmingToAction(lemming, false);

                    return true;
                }

                if (lemming.State.IsClimber && !WaterAt(lemming, orientation.MoveUp(lemmingPosition, 1)))
                {
                    // Only transition to climber, if the lemming is not under water
                    ClimberAction.Instance.TransitionLemmingToAction(lemming, false);

                    return true;
                }


                lemming.SetFacingDirection(lemming.FacingDirection.OppositeDirection());
                lemmingPosition = orientation.MoveLeft(lemmingPosition, dx); // Move lemming back
                lemming.LevelPosition = lemmingPosition;

                return true;
            }

            if (dy <= -3)
            {
                AscenderAction.Instance.TransitionLemmingToAction(lemming, false);
                lemmingPosition = orientation.MoveUp(lemmingPosition, 2);
                lemming.LevelPosition = lemmingPosition;

                return true;
            }

            if (dy <= -1 || (dy == 0 && !WaterAt(lemming, lemmingPosition)))
            {
                // see http://www.lemmingsforums.net/index.php?topic=3380.0
                // And the swimmer should not yet stop if the water and terrain overlaps

                WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
                lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
                lemming.LevelPosition = lemmingPosition;
            }

            return true;
        }

        // if no water or terrain on current position
        if (dy > 1)
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
            lemming.LevelPosition = lemmingPosition;
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        // if dy == 0 or == 1
        lemmingPosition = orientation.MoveDown(lemmingPosition, dy);
        lemming.LevelPosition = lemmingPosition;
        WalkerAction.Instance.TransitionLemmingToAction(lemming, false);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool WaterAt(Lemming lemming, LevelPosition lemmingPosition)
    {
        return GadgetCollections.Waters.TryGetGadgetThatMatchesTypeAndOrientation(lemming, lemmingPosition, out _);
    }

    /// <summary>
    /// Returns 0 if the lemming may not dive down. Otherwise return the amount of pixels the lemming dives
    /// </summary>
    private static int LemDive(
        Lemming lemming,
        LevelPosition lemmingPosition)
    {
        var result = 1;

        while (result <= 4 && Terrain.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveDown(lemmingPosition, result)))
        {
            result++;
            lemming.DistanceFallen++;

            if (WaterAt(lemming, lemming.Orientation.MoveDown(lemmingPosition, result)))
            {
                lemming.DistanceFallen = 0;
            }
        }

        return result > 4
            ? 0
            : result;
    }
}