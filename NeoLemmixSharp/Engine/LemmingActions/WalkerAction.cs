﻿using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class WalkerAction : ILemmingAction
{
    public const int NumberOfWalkerAnimationFrames = 8;

    public static WalkerAction Instance { get; } = new();

    private WalkerAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "walker";
    public int NumberOfAnimationFrames => NumberOfWalkerAnimationFrames;
    public bool IsOneTimeAction => false;

    public bool Equals(ILemmingAction? other) => other is WalkerAction;
    public override bool Equals(object? obj) => obj is WalkerAction;
    public override int GetHashCode() => nameof(WalkerAction).GetHashCode();

    public bool UpdateLemming(Lemming lemming)
    {
        var dx = lemming.FacingDirection.DeltaX;
        lemming.LevelPosition = lemming.Orientation.MoveRight(lemming.LevelPosition, dx);
        var dy = CommonMethods.FindGroundPixel(lemming.Orientation, lemming.LevelPosition);

        if (dy > 0 &&
            lemming.IsSlider &&
            CommonMethods.LemmingCanDehoist(lemming, true))
        {
            lemming.LevelPosition = lemming.Orientation.MoveLeft(lemming.LevelPosition, dx);
            CommonMethods.TransitionToNewAction(lemming, DehoisterAction.Instance, true);
            return true;
        }

        if (dy < -6)
        {
            if (lemming.IsClimber)
            {
                CommonMethods.TransitionToNewAction(lemming, ClimberAction.Instance, false);
            }
            else
            {
                lemming.FacingDirection = lemming.FacingDirection.OppositeDirection;
                lemming.LevelPosition = lemming.Orientation.MoveLeft(lemming.LevelPosition, dx);
            }
        }
        else if (dy < -2)
        {
            CommonMethods.TransitionToNewAction(lemming, AscenderAction.Instance, false);
            lemming.LevelPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, 2);
        }
        else if (dy < 1)
        {
            lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, dy);
        }

        // Get new ground pixel again in case the Lem has turned
        dy = CommonMethods.FindGroundPixel(lemming.Orientation, lemming.LevelPosition);

        if (dy > 3)
        {
            lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, 4);
            CommonMethods.TransitionToNewAction(lemming, FallerAction.Instance, false);
        }
        else if (dy > 0)
        {
            lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, dy);
        }

        return true;
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}