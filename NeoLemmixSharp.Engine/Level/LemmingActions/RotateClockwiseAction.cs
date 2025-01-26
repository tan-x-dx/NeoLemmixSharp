﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class RotateClockwiseAction : LemmingAction
{
    public static readonly RotateClockwiseAction Instance = new();

    private RotateClockwiseAction()
        : base(
            EngineConstants.RotateClockwiseActionId,
            EngineConstants.RotateClockwiseActionName,
            EngineConstants.RotateClockwiseActionSpriteFileName,
            EngineConstants.RotateClockwiseAnimationFrames,
            EngineConstants.MaxRotateClockwisePhysicsFrames,
            EngineConstants.NonWalkerMovementPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        if (lemming.EndOfAnimation)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            var orientation = lemming.Orientation;
            ref var lemmingPosition = ref lemming.LevelPosition;
            var dx = lemming.FacingDirection.DeltaX;
            lemmingPosition = orientation.Move(lemmingPosition, dx * -4, 4);
            lemming.SetOrientation(lemming.Orientation.RotateClockwise());
        }

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;
}