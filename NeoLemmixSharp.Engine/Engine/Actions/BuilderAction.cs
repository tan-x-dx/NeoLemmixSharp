﻿namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class BuilderAction : LemmingAction
{
    public const int NumberOfBuilderAnimationFrames = 16;

    public static BuilderAction Instance { get; } = new();

    private BuilderAction()
    {
    }

    public override int Id => 4;
    public override string LemmingActionName => "builder";
    public override int NumberOfAnimationFrames => NumberOfBuilderAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.AnimationFrame == 9)
        {
            LayBrick(lemming);
        }
        else if (lemming.AnimationFrame == 10 &&
                 lemming.NumberOfBricksLeft <= 3)
        {
            // play sound/make visual cue
        }
        else if (lemming.AnimationFrame == 0)
        {
            BuilderFrame0(lemming);
            lemming.ConstructivePositionFreeze = false;
        }

        return true;
    }

    private static void BuilderFrame0(Lemming lemming)
    {
        lemming.NumberOfBricksLeft--;

        var dx = lemming.FacingDirection.DeltaX;
        if (Terrain.PixelIsSolidToLemming(lemming.Orientation.Move(lemming.LevelPosition, dx, 2), lemming))
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);
        }
        else if (Terrain.PixelIsSolidToLemming(lemming.Orientation.Move(lemming.LevelPosition, dx, 3), lemming) ||
                 Terrain.PixelIsSolidToLemming(lemming.Orientation.Move(lemming.LevelPosition, dx + dx, 2), lemming) ||
                 (Terrain.PixelIsSolidToLemming(lemming.Orientation.Move(lemming.LevelPosition, dx + dx, 10), lemming) &&
                  lemming.NumberOfBricksLeft > 0))
        {
            lemming.LevelPosition = lemming.Orientation.Move(lemming.LevelPosition, dx, 1);
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);
        }
        else
        {
            if (!lemming.ConstructivePositionFreeze)
            {
                lemming.LevelPosition = lemming.Orientation.Move(lemming.LevelPosition, dx + dx, 1);
            }

            if (Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveUp(lemming.LevelPosition, 2), lemming) ||
                Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveUp(lemming.LevelPosition, 3), lemming) ||
                Terrain.PixelIsSolidToLemming(lemming.Orientation.Move(lemming.LevelPosition, dx, 3), lemming) ||
                (Terrain.PixelIsSolidToLemming(lemming.Orientation.Move(lemming.LevelPosition, dx + dx, 10), lemming) &&
                 lemming.NumberOfBricksLeft > 0))
            {
                WalkerAction.Instance.TransitionLemmingToAction(lemming, true);
            }
            else if (lemming.NumberOfBricksLeft == 0)
            {
                ShruggerAction.Instance.TransitionLemmingToAction(lemming, false);
            }
        }
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        lemming.NumberOfBricksLeft = 12;
        lemming.ConstructivePositionFreeze = false;
    }
}
