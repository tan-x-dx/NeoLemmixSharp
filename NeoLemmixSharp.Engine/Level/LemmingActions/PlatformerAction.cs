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
            EngineConstants.NonPermanentSkillPriority,
            LemmingActionBounds.PlatformerActionBounds)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        DoMainUpdate(lemming, in gadgetsNearLemming);

        if (lemming.PhysicsFrame == 0)
        {
            lemming.ConstructivePositionFreeze = false;
        }

        return true;
    }

    private static void DoMainUpdate(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        ref var lemmingPosition = ref lemming.AnchorPosition;

        if (lemming.PhysicsFrame == 9)
        {
            lemming.PlacedBrick = LemmingCanPlatform(lemming, gadgetsNearLemming);
            BuilderAction.LayBrick(lemming);

            return;
        }

        if (lemming.PhysicsFrame == 10 &&
            lemming.NumberOfBricksLeft <= EngineConstants.NumberOfRemainingBricksToPlaySound)
        {
            // ?? CueSoundEffect(SFX_BUILDER_WARNING, L.Position) ??
            return;
        }

        if (lemming.PhysicsFrame == 15)
        {
            if (!lemming.PlacedBrick)
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

            if (lemming.ConstructivePositionFreeze)
                return;

            lemmingPosition = orientation.MoveRight(lemmingPosition, dx);

            return;
        }

        if (lemming.PhysicsFrame != 0)
            return;

        if (PlatformerTerrainCheck(
                in gadgetsNearLemming,
                lemming,
                orientation.MoveRight(lemmingPosition, dx * 2)) &&
            lemming.NumberOfBricksLeft > 1)
        {
            lemmingPosition = orientation.MoveRight(lemmingPosition, dx);
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (PlatformerTerrainCheck(
                in gadgetsNearLemming,
                lemming,
                orientation.MoveRight(lemmingPosition, dx * 3)) &&
            lemming.NumberOfBricksLeft > 1)
        {
            lemmingPosition = orientation.MoveRight(lemmingPosition, dx * 2);
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (!lemming.ConstructivePositionFreeze)
        {
            lemmingPosition = orientation.MoveRight(lemmingPosition, dx * 2);
        }

        lemming.NumberOfBricksLeft--; // Why are we doing this here, instead at the beginning of frame 15??

        if (lemming.NumberOfBricksLeft != 0)
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
        var lemmingPosition = lemming.AnchorPosition;
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;

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
        return PositionIsSolidToLemming(in gadgetsNearLemming, lemming, lemming.Orientation.MoveUp(pos, 1)) ||
               PositionIsSolidToLemming(in gadgetsNearLemming, lemming, lemming.Orientation.MoveUp(pos, 2));
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        DoMainTransitionActions(lemming, turnAround);

        lemming.NumberOfBricksLeft = EngineConstants.NumberOfPlatformerBricks;
        lemming.ConstructivePositionFreeze = false;
    }
}
