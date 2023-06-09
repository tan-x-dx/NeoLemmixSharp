﻿namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class ClimberAction : LemmingAction
{
    public const int NumberOfClimberAnimationFrames = 8;

    public static ClimberAction Instance { get; } = new();

    private ClimberAction()
    {
    }

    public override int Id => 5;
    public override string LemmingActionName => "climber";
    public override int NumberOfAnimationFrames => NumberOfClimberAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => true;

    // Be very careful when changing the terrain/hoister checks for climbers!
    // See http://www.lemmingsforums.net/index.php?topic=2506.0 first!
    public override bool UpdateLemming(Lemming lemming)
    {
        var dx = lemming.FacingDirection.DeltaX;
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;
        var animationFrame = lemming.AnimationFrame;

        if (animationFrame <= 3)
        {
            var foundClip =
                Terrain.PixelIsSolidToLemming(orientation.Move(lemmingPosition, -dx, 6 + animationFrame), lemming) ||
                (Terrain.PixelIsSolidToLemming(orientation.Move(lemmingPosition, -dx, 5 + animationFrame), lemming) &&
                 !lemming.IsStartingAction);

            if (animationFrame == 0) // first triggered after 8 frames!
            {
                foundClip &= Terrain.PixelIsSolidToLemming(orientation.Move(lemmingPosition, -dx, 7), lemming);
            }

            if (foundClip)
            {
                // Don't fall below original position on hitting terrain in first cycle
                if (!lemming.IsStartingAction)
                {
                    lemmingPosition = orientation.MoveUp(lemmingPosition, 3 - animationFrame);
                    lemming.LevelPosition = lemmingPosition;
                }

                if (lemming.IsSlider)
                {
                    lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
                    lemming.LevelPosition = lemmingPosition;
                    SliderAction.Instance.TransitionLemmingToAction(lemming, false);
                }
                else
                {
                    lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
                    lemming.LevelPosition = lemmingPosition;
                    FallerAction.Instance.TransitionLemmingToAction(lemming, true);
                    lemming.DistanceFallen++; // Least-impact way to fix a fall distance inconsistency. See https://www.lemmingsforums.net/index.php?topic=5794.0
                }
            }
            else if (!Terrain.PixelIsSolidToLemming(orientation.MoveUp(lemmingPosition, 7 + animationFrame), lemming))
            {
                // if-case prevents too deep bombing, see http://www.lemmingsforums.net/index.php?topic=2620.0
                if (!(lemming.IsStartingAction && animationFrame == 1))
                {
                    lemmingPosition = orientation.MoveUp(lemmingPosition, 2 - animationFrame);
                    lemming.LevelPosition = lemmingPosition;
                    lemming.IsStartingAction = false;
                }

                HoisterAction.Instance.TransitionLemmingToAction(lemming, false);
            }
        }
        else
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
            lemming.LevelPosition = lemmingPosition;
            lemming.IsStartingAction = false;

            var foundClip = Terrain.PixelIsSolidToLemming(orientation.Move(lemmingPosition, -dx, 7), lemming);

            if (animationFrame == 7)
            {
                foundClip = foundClip && Terrain.PixelIsSolidToLemming(orientation.MoveUp(lemmingPosition, 7), lemming);
            }

            if (foundClip)
            {
                lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
                lemming.LevelPosition = lemmingPosition;

                if (lemming.IsSlider)
                {
                    SliderAction.Instance.TransitionLemmingToAction(lemming, false);
                }
                else
                {
                    lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
                    lemming.LevelPosition = lemmingPosition;
                    FallerAction.Instance.TransitionLemmingToAction(lemming, true);
                }
            }
        }

        return true;
    }
}