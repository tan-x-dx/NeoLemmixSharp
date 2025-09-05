using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class PlatformerAction : LemmingAction
{
    public static readonly PlatformerAction Instance = new();

    private PlatformerAction()
        : base(
            LemmingActionConstants.PlatformerActionId,
            LemmingActionConstants.PlatformerActionName,
            LemmingActionConstants.PlatformerActionSpriteFileName,
            LemmingActionConstants.PlatformerAnimationFrames,
            LemmingActionConstants.MaxPlatformerPhysicsFrames,
            LemmingActionConstants.NonPermanentSkillPriority,
            LemmingActionBounds.PlatformerActionBounds)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        DoMainUpdate(lemming, in gadgetsNearLemming);

        if (lemming.Data.PhysicsFrame == 0)
        {
            lemming.Data.ConstructivePositionFreeze = false;
        }

        return true;
    }

    private static void DoMainUpdate(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var orientation = lemming.Data.Orientation;
        var dx = lemming.Data.FacingDirection.DeltaX;
        ref var lemmingPosition = ref lemming.Data.AnchorPosition;

        if (lemming.Data.PhysicsFrame == 9)
        {
            lemming.Data.PlacedBrick = LemmingCanPlatform(lemming, gadgetsNearLemming);
            BuilderAction.LayBrick(lemming);

            return;
        }

        if (lemming.Data.PhysicsFrame == 10 &&
            lemming.Data.NumberOfBricksLeft <= EngineConstants.NumberOfRemainingBricksToPlaySound)
        {
            // ?? CueSoundEffect(SFX_BUILDER_WARNING, L.Position) ??
            return;
        }

        if (lemming.Data.PhysicsFrame == 15)
        {
            if (!lemming.Data.PlacedBrick)
            {
                WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

                return;
            }

            if (PlatformerTerrainCheck(
                    in gadgetsNearLemming,
                    lemming,
                    orientation.MoveRight(lemmingPosition, dx * 2)))
            {
                lemmingPosition = orientation.MoveRight(lemmingPosition, dx);

                WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

                return;
            }

            if (lemming.Data.ConstructivePositionFreeze)
                return;

            lemmingPosition = orientation.MoveRight(lemmingPosition, dx);

            return;
        }

        if (lemming.Data.PhysicsFrame != 0)
            return;

        if (PlatformerTerrainCheck(
                in gadgetsNearLemming,
                lemming,
                orientation.MoveRight(lemmingPosition, dx * 2)) &&
            lemming.Data.NumberOfBricksLeft > 1)
        {
            lemmingPosition = orientation.MoveRight(lemmingPosition, dx);
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (PlatformerTerrainCheck(
                in gadgetsNearLemming,
                lemming,
                orientation.MoveRight(lemmingPosition, dx * 3)) &&
            lemming.Data.NumberOfBricksLeft > 1)
        {
            lemmingPosition = orientation.MoveRight(lemmingPosition, dx * 2);
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (!lemming.Data.ConstructivePositionFreeze)
        {
            lemmingPosition = orientation.MoveRight(lemmingPosition, dx * 2);
        }

        lemming.Data.NumberOfBricksLeft--; // Why are we doing this here, instead at the beginning of frame 15??

        if (lemming.Data.NumberOfBricksLeft != 0)
            return;

        // stalling if there are pixels in the way:
        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 1)))
        {
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
        }

        ShruggerAction.Instance.TransitionLemmingToAction(lemming, false);
    }

    public static bool LemmingCanPlatform(
        Lemming lemming,
        in GadgetEnumerable gadgetsNearLemming)
    {
        var lemmingPosition = lemming.Data.AnchorPosition;
        var orientation = lemming.Data.Orientation;
        var dx = lemming.Data.FacingDirection.DeltaX;

        var result = !PositionIsSolidToLemming(in gadgetsNearLemming, lemming, lemmingPosition) ||
                     !PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveRight(lemmingPosition, dx)) ||
                     !PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveRight(lemmingPosition, dx * 2)) ||
                     !PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveRight(lemmingPosition, dx * 3)) ||
                     !PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveRight(lemmingPosition, dx * 4));

        result = result && !PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx, 1));
        result = result && !PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, dx * 2, 1));
        return result;
    }

    private static bool PlatformerTerrainCheck(
        in GadgetEnumerable gadgetsNearLemming,
        Lemming lemming,
        Point pos)
    {
        return PositionIsSolidToLemming(in gadgetsNearLemming, lemming, lemming.Data.Orientation.MoveUp(pos, 1)) ||
               PositionIsSolidToLemming(in gadgetsNearLemming, lemming, lemming.Data.Orientation.MoveUp(pos, 2));
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        DoMainTransitionActions(lemming, turnAround);

        lemming.Data.NumberOfBricksLeft = EngineConstants.NumberOfPlatformerBricks;
        lemming.Data.ConstructivePositionFreeze = false;
    }
}
