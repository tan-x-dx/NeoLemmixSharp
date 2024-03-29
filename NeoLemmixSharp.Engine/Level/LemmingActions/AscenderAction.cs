﻿using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class AscenderAction : LemmingAction
{
    public static readonly AscenderAction Instance = new();

    private AscenderAction()
    {
    }

    public override int Id => LevelConstants.AscenderActionId;
    public override string LemmingActionName => "ascender";
    public override int NumberOfAnimationFrames => LevelConstants.AscenderAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => LevelConstants.NonWalkerMovementPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        var terrainManager = LevelScreen.TerrainManager;
        ref var levelPosition = ref lemming.LevelPosition;
        var orientation = lemming.Orientation;

        var dy = 0;
        while (dy < 2 &&
               lemming.AscenderProgress < 5 &&
               terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(levelPosition, 1)))
        {
            dy++;
            levelPosition = orientation.MoveUp(levelPosition, 1);
            lemming.AscenderProgress++;
        }

        var pixel1IsSolid = terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(levelPosition, 1));
        var pixel2IsSolid = terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(levelPosition, 2));

        if (dy < 2 &&
            !pixel1IsSolid)
        {
            lemming.SetNextAction(WalkerAction.Instance);
            return true;
        }
        
        if ((lemming.AscenderProgress == 4 &&
                  pixel1IsSolid &&
                  pixel2IsSolid) ||
                 (lemming.AscenderProgress >= 5 &&
                  pixel1IsSolid))
        {
            var dx = lemming.FacingDirection.DeltaX;
            lemming.LevelPosition = orientation.MoveLeft(levelPosition, dx);
            FallerAction.Instance.TransitionLemmingToAction(lemming, true);
        }

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -4;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 2;
    protected override int BottomRightBoundsDeltaY(int animationFrame) => 0;

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        lemming.AscenderProgress = 0;
    }
}