using System.Runtime.CompilerServices;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class DehoisterAction : LemmingAction
{
    public static readonly DehoisterAction Instance = new();

    private DehoisterAction()
        : base(
            LevelConstants.DehoisterActionId,
            LevelConstants.DehoisterActionName,
            LevelConstants.DehoisterAnimationFrames,
            LevelConstants.MaxDehoisterPhysicsFrames,
            LevelConstants.NonWalkerMovementPriority,
            true,
            false)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;

        var gadgetsNearRegion =
            LevelScreen.GadgetManager.GetAllGadgetsForPosition(orientation.MoveUp(lemmingPosition, 7));

        if (lemming.EndOfAnimation)
        {
            if (PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 7)))
            {
                SliderAction.Instance.TransitionLemmingToAction(lemming, false);
                return true;
            }

            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        if (lemming.PhysicsFrame < 2)
            return true;

        lemmingPosition = orientation.MoveDown(lemmingPosition, 1);

        var animFrameValue = lemming.PhysicsFrame * 2;

        if (!SliderAction.SliderTerrainChecks(lemming, orientation, animFrameValue - 3) &&
            lemming.CurrentAction == DrownerAction.Instance)
            return false;

        lemmingPosition = orientation.MoveDown(lemmingPosition, 1);

        return SliderAction.SliderTerrainChecks(lemming, orientation, animFrameValue - 2) ||
               lemming.CurrentAction != DrownerAction.Instance;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -5;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 11;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 1;
    protected override int BottomRightBoundsDeltaY(int animationFrame) => 0;

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        lemming.DehoistPin = lemming.LevelPosition;

        base.TransitionLemmingToAction(lemming, turnAround);
    }

    [SkipLocalsInit]
    public static bool LemmingCanDehoist(
        Lemming lemming,
        bool alreadyMoved)
    {
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        LevelPosition currentPosition;
        LevelPosition nextPosition;
        if (alreadyMoved)
        {
            nextPosition = lemming.LevelPosition;
            currentPosition = orientation.MoveLeft(nextPosition, dx);
        }
        else
        {
            currentPosition = lemming.LevelPosition;
            nextPosition = orientation.MoveRight(currentPosition, dx);
        }

        // Subroutine of other LevelAction methods.
        // Use a dummy scratch space span to prevent data from being overridden.
        // Prevents weird bugs!
        Span<uint> scratchSpace = stackalloc uint[LevelScreen.GadgetManager.ScratchSpaceSize];

        var gadgetTestRegion = new LevelPositionPair(
            orientation.Move(nextPosition, dx, 1),
            orientation.Move(nextPosition, -dx, -4));
        var gadgetsNearRegion = LevelScreen.GadgetManager.GetAllItemsNearRegion(scratchSpace, gadgetTestRegion);

        if (LevelScreen.TerrainManager.PositionOutOfBounds(nextPosition) ||
            !PositionIsSolidToLemming(gadgetsNearRegion, lemming, currentPosition) ||
            PositionIsSolidToLemming(gadgetsNearRegion, lemming, nextPosition))
            return false;

        for (var i = 1; i < 4; i++)
        {
            if (PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.MoveDown(nextPosition, i)))
                return false;
            if (!PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.MoveDown(currentPosition, i)))
                return true;
        }

        return true;
    }
}