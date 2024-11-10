using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Runtime.CompilerServices;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class DehoisterAction : LemmingAction
{
    public static readonly DehoisterAction Instance = new();

    private DehoisterAction()
        : base(
            EngineConstants.DehoisterActionId,
            EngineConstants.DehoisterActionName,
            EngineConstants.DehoisterActionSpriteFileName,
            EngineConstants.DehoisterAnimationFrames,
            EngineConstants.MaxDehoisterPhysicsFrames,
            EngineConstants.NonWalkerMovementPriority)
    {
    }

    [SkipLocalsInit]
    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;

        var gadgetManager = LevelScreen.GadgetManager;
        Span<uint> scratchSpaceSpan = stackalloc uint[gadgetManager.ScratchSpaceSize];
        gadgetManager.GetAllGadgetsForPosition(
            scratchSpaceSpan,
            orientation.MoveUp(lemmingPosition, 7),
            out var gadgetsNearRegion);

        if (lemming.EndOfAnimation)
        {
            if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 7)))
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

        var gadgetManager = LevelScreen.GadgetManager;
        Span<uint> scratchSpace = stackalloc uint[gadgetManager.ScratchSpaceSize];

        var gadgetTestRegion = new LevelRegion(
            orientation.Move(nextPosition, dx, 1),
            orientation.Move(nextPosition, -dx, -4));
        gadgetManager.GetAllItemsNearRegion(scratchSpace, gadgetTestRegion, out var gadgetsNearRegion);

        if (LevelScreen.PositionOutOfBounds(nextPosition) ||
            !PositionIsSolidToLemming(in gadgetsNearRegion, lemming, currentPosition) ||
            PositionIsSolidToLemming(in gadgetsNearRegion, lemming, nextPosition))
            return false;

        for (var i = 1; i < 4; i++)
        {
            if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveDown(nextPosition, i)))
                return false;
            if (!PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveDown(currentPosition, i)))
                return true;
        }

        return true;
    }
}