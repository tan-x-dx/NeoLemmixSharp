using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class WalkerAction : LemmingAction
{
    public const int NumberOfWalkerAnimationFrames = 8;

    public static WalkerAction Instance { get; } = new();

    private WalkerAction()
    {
    }

    public override int Id => 0;
    public override string LemmingActionName => "walker";
    public override int NumberOfAnimationFrames => NumberOfWalkerAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        var lemmingPosition = lemming.LevelPosition;
        lemmingPosition = orientation.MoveRight(lemmingPosition, dx);
        lemming.LevelPosition = lemmingPosition;
        var dy = FindGroundPixel(lemming, orientation, lemmingPosition);

        if (dy > 0 &&
            lemming.IsSlider &&
            LemmingCanDehoist(lemming, true))
        {
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
            lemming.LevelPosition = lemmingPosition;
            DehoisterAction.Instance.TransitionLemmingToAction(lemming, true);
            return true;
        }

        if (dy < -6)
        {
            if (lemming.IsClimber)
            {
                ClimberAction.Instance.TransitionLemmingToAction(lemming, false);
            }
            else
            {
                lemming.SetFacingDirection(lemming.FacingDirection.OppositeDirection);
                lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
                lemming.LevelPosition = lemmingPosition;
            }
        }
        else if (dy < -2)
        {
            AscenderAction.Instance.TransitionLemmingToAction(lemming, false);
            lemmingPosition = orientation.MoveUp(lemmingPosition, 2);
            lemming.LevelPosition = lemmingPosition;
        }
        else if (dy < 1)
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, dy);
            lemming.LevelPosition = lemmingPosition;
        }

        // Get new ground pixel again in case the Lem has turned
        dy = FindGroundPixel(lemming, orientation, lemmingPosition);

        if (dy > 3)
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 4);
            lemming.LevelPosition = lemmingPosition;
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
        }
        else if (dy > 0)
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, dy);
            lemming.LevelPosition = lemmingPosition;
        }

        return true;
    }

    private static bool LemmingCanDehoist(Lemming lemming, bool alreadyMoved)
    {
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        LevelPosition currentPosition;
        LevelPosition nextPosition;
        if (alreadyMoved)
        {
            nextPosition = lemming.LevelPosition;
            currentPosition = orientation.MoveLeft(nextPosition, dx);
        }
        else
        {
            currentPosition = lemming.LevelPosition;
            nextPosition = orientation.MoveRight(currentPosition, dx);
        }

        if (Terrain.PositionOutOfBounds(nextPosition) ||
            (!Terrain.PixelIsSolidToLemming(currentPosition, lemming) ||
             Terrain.PixelIsSolidToLemming(nextPosition, lemming)))
            return false;

        if (Terrain.PixelIsSolidToLemming(orientation.MoveDown(nextPosition, 1), lemming))
            return false;
        if (!Terrain.PixelIsSolidToLemming(orientation.MoveDown(currentPosition, 1), lemming))
            return true;

        if (Terrain.PixelIsSolidToLemming(orientation.MoveDown(nextPosition, 2), lemming))
            return false;
        if (!Terrain.PixelIsSolidToLemming(orientation.MoveDown(currentPosition, 2), lemming))
            return true;

        if (Terrain.PixelIsSolidToLemming(orientation.MoveDown(nextPosition, 3), lemming))
            return false;
        if (!Terrain.PixelIsSolidToLemming(orientation.MoveDown(currentPosition, 3), lemming))
            return true;

        if (Terrain.PixelIsSolidToLemming(orientation.MoveDown(nextPosition, 4), lemming))
            return false;
        return !Terrain.PixelIsSolidToLemming(orientation.MoveDown(currentPosition, 4), lemming);
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        if (Terrain.PixelIsSolidToLemming(lemming.LevelPosition, lemming))
        {
            base.TransitionLemmingToAction(lemming, turnAround);
            return;
        }

        FallerAction.Instance.TransitionLemmingToAction(lemming, turnAround);
    }
}