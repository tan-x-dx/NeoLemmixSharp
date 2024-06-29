﻿using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class RotateClockwiseAction : LemmingAction
{
    public static readonly RotateClockwiseAction Instance = new();

    private RotateClockwiseAction()
        : base(
            LevelConstants.RotateClockwiseActionId, 
            LevelConstants.RotateClockwiseActionName,
            LevelConstants.RotateClockwiseAnimationFrames,
            LevelConstants.MaxRotateClockwisePhysicsFrames,
            LevelConstants.NonWalkerMovementPriority,
            true, 
            true)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            lemming.SetOrientation(Orientation.RotateClockwise(lemming.Orientation));
        }

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;
}