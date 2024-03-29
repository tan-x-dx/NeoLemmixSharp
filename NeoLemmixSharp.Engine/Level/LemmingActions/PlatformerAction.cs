﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class PlatformerAction : LemmingAction
{
    public static readonly PlatformerAction Instance = new();

    private PlatformerAction()
    {
    }

    public override int Id => LevelConstants.PlatformerActionId;
    public override string LemmingActionName => "platformer";
    public override int NumberOfAnimationFrames => LevelConstants.PlatformerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => LevelConstants.NonPermanentSkillPriority;

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
        var terrainManager = LevelScreen.TerrainManager;
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        ref var lemmingPosition = ref lemming.LevelPosition;

        if (lemming.PhysicsFrame == 9)
        {
            lemming.PlacedBrick = LemmingCanPlatform(lemming, orientation);
            BuilderAction.LayBrick(lemming);

            return;
        }

        if (lemming.PhysicsFrame == 10 && lemming.NumberOfBricksLeft <= 3)
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

            if (PlatformerTerrainCheck(lemming, orientation.MoveRight(lemmingPosition, dx + dx)))
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

        if (PlatformerTerrainCheck(lemming, orientation.MoveRight(lemmingPosition, dx + dx)) &&
            lemming.NumberOfBricksLeft > 1)
        {
            lemmingPosition = orientation.MoveRight(lemmingPosition, dx);
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (PlatformerTerrainCheck(lemming, orientation.MoveRight(lemmingPosition, dx + dx + dx)) &&
            lemming.NumberOfBricksLeft > 1)
        {
            lemmingPosition = orientation.MoveRight(lemmingPosition, dx + dx);
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (!lemming.ConstructivePositionFreeze)
        {
            lemmingPosition = orientation.MoveRight(lemmingPosition, dx + dx);
        }

        lemming.NumberOfBricksLeft--; // Why are we doing this here, instead at the beginning of frame 15??

        if (lemming.NumberOfBricksLeft != 0)
            return;

        // stalling if there are pixels in the way:
        if (terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 1)))
        {
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
        }

        ShruggerAction.Instance.TransitionLemmingToAction(lemming, false);
    }

    public static bool LemmingCanPlatform(
        Lemming lemming,
        Orientation orientation)
    {
        var terrainManager = LevelScreen.TerrainManager;
        var lemmingPosition = lemming.LevelPosition;

        var result = !terrainManager.PixelIsSolidToLemming(lemming, lemmingPosition) ||
                     !terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveRight(lemmingPosition, 1)) ||
                     !terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveRight(lemmingPosition, 2)) ||
                     !terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveRight(lemmingPosition, 3)) ||
                     !terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveRight(lemmingPosition, 4));

        var dx = lemming.FacingDirection.DeltaX;
        result = result && !terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx, 1));
        result = result && !terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx + dx, 1));
        return result;
    }

    private static bool PlatformerTerrainCheck(
        Lemming lemming,
        LevelPosition pos)
    {
        var terrainManager = LevelScreen.TerrainManager;
        return terrainManager.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveUp(pos, 1)) ||
               terrainManager.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveUp(pos, 2));
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        lemming.NumberOfBricksLeft = 12;
        lemming.ConstructivePositionFreeze = false;
    }
}