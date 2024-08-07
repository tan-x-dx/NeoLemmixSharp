﻿using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class RotateHalfAction : LemmingAction
{
    public static readonly RotateHalfAction Instance = new();

    private RotateHalfAction()
        : base(
            LevelConstants.RotateHalfActionId,
            LevelConstants.RotateHalfActionName,
            LevelConstants.RotateHalfActionSpriteFileName,
            LevelConstants.RotateHalfAnimationFrames,
            LevelConstants.MaxRotateHalfPhysicsFrames,
            LevelConstants.NonWalkerMovementPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            var orientation = lemming.Orientation;
            ref var lemmingPosition = ref lemming.LevelPosition;
            lemmingPosition = orientation.MoveUp(lemmingPosition, 8);
            lemming.SetOrientation(Orientation.GetOpposite(orientation));
        }

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;
}