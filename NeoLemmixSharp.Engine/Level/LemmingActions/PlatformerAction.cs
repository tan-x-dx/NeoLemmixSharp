using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class PlatformerAction : LemmingAction
{
    public static readonly PlatformerAction Instance = new();

    private PlatformerAction()
        : base(
            LevelConstants.PlatformerActionId,
            LevelConstants.PlatformerActionName,
            LevelConstants.PlatformerAnimationFrames,
            LevelConstants.MaxPlatformerPhysicsFrames,
            LevelConstants.NonPermanentSkillPriority,
            false,
            false)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        DoMainUpdate(lemming);

        if (lemming.PhysicsFrame == 0)
        {
            lemming.ConstructivePositionFreeze = false;
        }

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => animationFrame switch
    {
        13 => -2,
        14 => -1,
        15 => -1,
        _ => -3
    };

    protected override int TopLeftBoundsDeltaY(int animationFrame) => 8;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => animationFrame switch
    {
        13 => 4,
        14 => 5,
        15 => 5,
        _ => 3
    };

    private static void DoMainUpdate(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        ref var lemmingPosition = ref lemming.LevelPosition;

        var gadgetTestRegion = new LevelPositionPair(
            lemmingPosition,
            orientation.Move(lemmingPosition, dx * 4, 2));
        var gadgetsNearRegion = LevelScreen.GadgetManager.GetAllItemsNearRegion(gadgetTestRegion);

        if (lemming.PhysicsFrame == 9)
        {
            lemming.PlacedBrick = LemmingCanPlatform(lemming);
            BuilderAction.LayBrick(lemming);

            return;
        }

        if (lemming.PhysicsFrame == 10 &&
            lemming.NumberOfBricksLeft <= LevelConstants.NumberOfRemainingBricksToPlaySound)
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
                    in gadgetsNearRegion,
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
                in gadgetsNearRegion,
                lemming,
                orientation.MoveRight(lemmingPosition, dx * 2)) &&
            lemming.NumberOfBricksLeft > 1)
        {
            lemmingPosition = orientation.MoveRight(lemmingPosition, dx);
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (PlatformerTerrainCheck(
                in gadgetsNearRegion,
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
        if (PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 1)))
        {
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
        }

        ShruggerAction.Instance.TransitionLemmingToAction(lemming, false);
    }

    public static bool LemmingCanPlatform(
        Lemming lemming)
    {
        var lemmingPosition = lemming.LevelPosition;
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;

        // Subroutine of other LevelAction methods.
        // Use a dummy scratch space span to prevent data from being overridden.
        // Prevents weird bugs!
        Span<uint> scratchSpace = stackalloc uint[LevelScreen.GadgetManager.ScratchSpaceSize];

        var gadgetTestRegion = new LevelPositionPair(
            lemmingPosition,
            orientation.Move(lemmingPosition, dx * 4, 1));
        var gadgetsNearRegion = LevelScreen.GadgetManager.GetAllItemsNearRegion(scratchSpace, gadgetTestRegion);

        var result = !PositionIsSolidToLemming(gadgetsNearRegion, lemming, lemmingPosition) ||
                     !PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.MoveRight(lemmingPosition, dx)) ||
                     !PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.MoveRight(lemmingPosition, dx * 2)) ||
                     !PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.MoveRight(lemmingPosition, dx * 3)) ||
                     !PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.MoveRight(lemmingPosition, dx * 4));

        result = result && !PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.Move(lemmingPosition, dx, 1));
        result = result && !PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.Move(lemmingPosition, dx * 2, 1));
        return result;
    }

    private static bool PlatformerTerrainCheck(
        in GadgetSet gadgetsNearRegion,
        Lemming lemming,
        LevelPosition pos)
    {
        return PositionIsSolidToLemming(gadgetsNearRegion, lemming, lemming.Orientation.MoveUp(pos, 1)) ||
               PositionIsSolidToLemming(gadgetsNearRegion, lemming, lemming.Orientation.MoveUp(pos, 2));
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        lemming.NumberOfBricksLeft = LevelConstants.NumberOfPlatformerBricks;
        lemming.ConstructivePositionFreeze = false;
    }
}