﻿namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class ShimmierAction : LemmingAction
{
    public const int NumberOfShimmierAnimationFrames = 20;

    public static ShimmierAction Instance { get; } = new();

    private ShimmierAction()
    {
    }

    public override int Id => 14;
    public override string LemmingActionName => "shimmier";
    public override int NumberOfAnimationFrames => NumberOfShimmierAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;
        var dx = lemming.FacingDirection.DeltaX;

        if ((lemming.AnimationFrame & 1) == 0)
        {
            var i = 0;
            // Check whether we find terrain to walk onto
            for (; i < 3; i++)
            {
                if (Terrain.PixelIsSolidToLemming(orientation.Move(lemmingPosition, dx, i), lemming) &&
                    !Terrain.PixelIsSolidToLemming(orientation.Move(lemmingPosition, dx, i + 1), lemming))
                {
                    lemmingPosition = orientation.Move(lemmingPosition, dx, i);
                    lemming.LevelPosition = lemmingPosition;
                    WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
                    return true;
                }
            }

            // Check whether we find terrain to hoist onto
            for (; i < 6; i++)
            {
                if (Terrain.PixelIsSolidToLemming(orientation.Move(lemmingPosition, dx, i), lemming) &&
                    !Terrain.PixelIsSolidToLemming(orientation.Move(lemmingPosition, dx, i + 1), lemming))
                {
                    lemmingPosition = orientation.Move(lemmingPosition, dx, i - 4);
                    lemming.LevelPosition = lemmingPosition;
                    lemming.IsStartingAction = false;
                    HoisterAction.Instance.TransitionLemmingToAction(lemming, false);
                    lemming.AnimationFrame += 2;
                    return true;
                }
            }

            // Check whether we fall down due to a wall
            for (; i < 8; i++)
            {
                if (Terrain.PixelIsSolidToLemming(orientation.Move(lemmingPosition, dx, i), lemming))
                {
                    if (lemming.IsSlider)
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
        }

        var pixel9AboveIsSolid = Terrain.PixelIsSolidToLemming(orientation.Move(lemmingPosition, dx, 9), lemming);
        // Check whether we fall down due to not enough ceiling terrain
        if (!pixel9AboveIsSolid &&
            !Terrain.PixelIsSolidToLemming(orientation.Move(lemmingPosition, dx, 10), lemming))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        // Check whether we fall down due a checkerboard ceiling
        if (Terrain.PixelIsSolidToLemming(orientation.Move(lemmingPosition, dx, 8), lemming) &&
            !pixel9AboveIsSolid)
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        // Move along
        lemmingPosition = orientation.MoveRight(lemmingPosition, dx);
        lemming.LevelPosition = lemmingPosition;

        if (Terrain.PixelIsSolidToLemming(orientation.MoveUp(lemmingPosition, 8), lemming))
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
            lemming.LevelPosition = lemmingPosition;

            if (Terrain.PixelIsSolidToLemming(lemmingPosition, lemming))
            {
                WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
                return true;
            }

            var bottomOfLevel = orientation.BottomLeftCornerOfLevel();
            if (orientation.FirstIsBelowSecond(lemmingPosition, orientation.MoveDown(bottomOfLevel, 8)))
            {
                // ?? RemoveLemming(L, RM_NEUTRAL); ??
                return true;
            }
        }

        if (Terrain.PixelIsSolidToLemming(orientation.MoveUp(lemmingPosition, 9), lemming))
            return true;

        lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
        lemming.LevelPosition = lemmingPosition;

        lemmingPosition = orientation.MoveUp(lemmingPosition, 5);
        if (Terrain.PixelIsSolidToLemming(lemmingPosition, lemming))
        {
            lemming.LevelPosition = lemmingPosition;
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
        }

        return true;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;

        if (lemming.CurrentAction == ClimberAction.Instance)
        {
            lemming.SetFacingDirection(lemming.FacingDirection.OppositeDirection);
            lemmingPosition = orientation.MoveRight(lemmingPosition, lemming.FacingDirection.DeltaX);
            lemming.LevelPosition = lemmingPosition;

            if (Terrain.PixelIsSolidToLemming(orientation.MoveUp(lemmingPosition, 8), lemming))
            {
                lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
                lemming.LevelPosition = lemmingPosition;
            }
        }
        else if (lemming.CurrentAction == SliderAction.Instance ||
                 lemming.CurrentAction == DehoisterAction.Instance)
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 2);
            lemming.LevelPosition = lemmingPosition;
            if (Terrain.PixelIsSolidToLemming(orientation.MoveUp(lemmingPosition, 8), lemming))
            {
                lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
                lemming.LevelPosition = lemmingPosition;
            }
        }
        else if (lemming.CurrentAction == JumperAction.Instance)
        {
            for (var i = -1; i < 4; i++)
            {
                if (Terrain.PixelIsSolidToLemming(orientation.MoveUp(lemmingPosition, 9 + i), lemming) &&
                    !Terrain.PixelIsSolidToLemming(orientation.MoveUp(lemmingPosition, 8 + i), lemming))
                {
                    lemmingPosition = orientation.MoveUp(lemmingPosition, i);
                    lemming.LevelPosition = lemmingPosition;
                }
            }
        }

        base.TransitionLemmingToAction(lemming, turnAround);
    }
}