﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class ShimmierAction : LemmingAction
{
    public static readonly ShimmierAction Instance = new();

    private ShimmierAction()
        : base(
            LevelConstants.ShimmierActionId,
            LevelConstants.ShimmierActionName,
            LevelConstants.ShimmierActionSpriteFileName,
            LevelConstants.ShimmierAnimationFrames,
            LevelConstants.MaxShimmierPhysicsFrames,
            LevelConstants.NonWalkerMovementPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;
        var dx = lemming.FacingDirection.DeltaX;

        if ((lemming.PhysicsFrame & 1) != 0)
            return true;

        var gadgetTestRegion = new LevelPositionPair(
            lemmingPosition,
            orientation.Move(lemmingPosition, dx, 10));
        LevelScreen.GadgetManager.GetAllItemsNearRegion(gadgetTestRegion, out var gadgetsNearRegion);

        var i = 0;
        // Check whether we find terrain to walk onto
        for (; i < 3; i++)
        {
            if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.Move(lemmingPosition, dx, i)) &&
                !PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.Move(lemmingPosition, dx, i + 1)))
            {
                lemmingPosition = orientation.Move(lemmingPosition, dx, i);
                WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
                return true;
            }
        }

        // Check whether we find terrain to hoist onto
        for (; i < 6; i++)
        {
            if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.Move(lemmingPosition, dx, i)) &&
                !PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.Move(lemmingPosition, dx, i + 1)))
            {
                lemmingPosition = orientation.Move(lemmingPosition, dx, i - 4);
                lemming.IsStartingAction = false;
                HoisterAction.Instance.TransitionLemmingToAction(lemming, false);
                lemming.PhysicsFrame += 2;
                lemming.AnimationFrame += 2;
                return true;
            }
        }

        // Check whether we fall down due to a wall
        for (; i < 8; i++)
        {
            if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.Move(lemmingPosition, dx, i)))
            {
                if (lemming.State.IsSlider)
                {
                    SliderAction.Instance.TransitionLemmingToAction(lemming, false);
                }
                else
                {
                    FallerAction.Instance.TransitionLemmingToAction(lemming, false);
                }

                return true;
            }
        }

        var pixel9AboveIsSolid = PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.Move(lemmingPosition, dx, 9));
        // Check whether we fall down due to not enough ceiling terrain
        if (!pixel9AboveIsSolid &&
            !PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.Move(lemmingPosition, dx, 10)))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        // Check whether we fall down due a checkerboard ceiling
        if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.Move(lemmingPosition, dx, 8)) &&
            !pixel9AboveIsSolid)
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        // Move along
        lemmingPosition = orientation.MoveRight(lemmingPosition, dx);

        if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 8)))
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 1);

            if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, lemmingPosition))
            {
                WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
                return true;
            }

            if (LevelScreen.PositionOutOfBounds(lemmingPosition))
            {
                LevelScreen.LemmingManager.RemoveLemming(lemming, LemmingRemovalReason.DeathVoid);
                return true;
            }
        }

        if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 9)))
            return true;

        lemmingPosition = orientation.MoveUp(lemmingPosition, 1);

        var checkPosition = orientation.MoveUp(lemmingPosition, 5);
        if (!PositionIsSolidToLemming(in gadgetsNearRegion, lemming, checkPosition))
            return true;

        lemmingPosition = checkPosition;
        WalkerAction.Instance.TransitionLemmingToAction(lemming, false);

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 9;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;
    protected override int BottomRightBoundsDeltaY(int animationFrame) => -2;

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;
        var dx = lemming.FacingDirection.DeltaX;

        var gadgetTestRegion = new LevelPositionPair(
            lemmingPosition,
            orientation.Move(lemmingPosition, dx, 12));
        LevelScreen.GadgetManager.GetAllItemsNearRegion(gadgetTestRegion, out var gadgetsNearRegion);

        if (lemming.CurrentAction == ClimberAction.Instance)
        {
            lemming.SetFacingDirection(lemming.FacingDirection.GetOpposite());
            lemmingPosition = orientation.MoveRight(lemmingPosition, dx);

            if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 8)))
            {
                lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
            }
        }
        else if (lemming.CurrentAction == SliderAction.Instance ||
                 lemming.CurrentAction == DehoisterAction.Instance)
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 2);
            if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 8)))
            {
                lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
            }
        }
        else if (lemming.CurrentAction == JumperAction.Instance)
        {
            for (var i = -1; i < 4; i++)
            {
                if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 9 + i)) &&
                    !PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 8 + i)))
                {
                    lemmingPosition = orientation.MoveUp(lemmingPosition, i);
                }
            }
        }

        base.TransitionLemmingToAction(lemming, turnAround);
    }
}