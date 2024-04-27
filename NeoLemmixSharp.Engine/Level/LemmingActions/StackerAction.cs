﻿using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class StackerAction : LemmingAction
{
    public static readonly StackerAction Instance = new();

    private StackerAction()
        : base(
            LevelConstants.StackerActionId,
            LevelConstants.StackerActionName,
            LevelConstants.StackerAnimationFrames,
            LevelConstants.MaxStackerPhysicsFrames,
            LevelConstants.NonPermanentSkillPriority,
            false,
            false)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.PhysicsFrame == LevelConstants.StackerAnimationFrames - 1)
        {
            lemming.PlacedBrick = LayStackBrick(lemming);
        }
        else if (lemming.PhysicsFrame == 0)
        {
            lemming.NumberOfBricksLeft--;

            if (lemming.NumberOfBricksLeft < LevelConstants.NumberOfRemainingBricksToPlaySound)
            {
                // ?? CueSoundEffect(SFX_BUILDER_WARNING, L.Position); ??
            }

            if (!lemming.PlacedBrick)
            {
                // Relax the check on the first brick
                // for details see http://www.lemmingsforums.net/index.php?topic=2862.0
                if (lemming.NumberOfBricksLeft < LevelConstants.NumberOfStackerBricks - 1 ||
                    !MayPlaceNextBrick(lemming))
                {
                    WalkerAction.Instance.TransitionLemmingToAction(lemming, true);
                }
            }
            else if (lemming.NumberOfBricksLeft == 0)
            {
                ShruggerAction.Instance.TransitionLemmingToAction(lemming, false);
            }
        }

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -2;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;

    private static bool MayPlaceNextBrick(Lemming lemming)
    {
        var terrainManager = LevelScreen.TerrainManager;
        var orientation = lemming.Orientation;
        var brickPosition = lemming.LevelPosition;
        brickPosition = orientation.MoveUp(brickPosition, 1 + LevelConstants.NumberOfStackerBricks - lemming.NumberOfBricksLeft);

        var dx = lemming.FacingDirection.DeltaX;

        return !(terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveRight(brickPosition, dx)) &&
                 terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveRight(brickPosition, dx + dx)) &&
                 terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveRight(brickPosition, dx + dx + dx)));
    }

    private static bool LayStackBrick(Lemming lemming)
    {
        var terrainManager = LevelScreen.TerrainManager;
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        var dy = lemming.StackLow ? -1 : 0;
        var brickPosition = orientation.Move(lemming.LevelPosition, dx, 1 + LevelConstants.NumberOfStackerBricks + dy - lemming.NumberOfBricksLeft);

        var result = false;

        for (var i = 0; i < 3; i++)
        {
            if (!terrainManager.PixelIsSolidToLemming(lemming, brickPosition))
            {
                terrainManager.SetSolidPixel(brickPosition, uint.MaxValue);
                result = true;
            }

            brickPosition = orientation.MoveRight(brickPosition, dx);
        }

        return result;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        lemming.NumberOfBricksLeft = LevelConstants.NumberOfStackerBricks;
    }
}