using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class WalkerAction : LemmingAction
{
    public static WalkerAction Instance { get; } = new();

    private WalkerAction()
    {
    }

    public override int Id => 0;
    public override string LemmingActionName => "walker";
    public override int NumberOfAnimationFrames => GameConstants.WalkerAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        var lemmingPosition = lemming.LevelPosition;

        lemmingPosition = orientation.MoveRight(lemmingPosition, dx);
        lemming.LevelPosition = lemmingPosition;
        var dy = FindGroundPixel(orientation, lemmingPosition);

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
                lemming.SetFacingDirection(lemming.FacingDirection.OppositeDirection());
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
        dy = FindGroundPixel(orientation, lemmingPosition);

        if (dy > 3)
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 4);
            lemming.LevelPosition = lemmingPosition;
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        if (dy <= 0)
            return true;

        lemmingPosition = orientation.MoveDown(lemmingPosition, dy);
        lemming.LevelPosition = lemmingPosition;

        return true;
    }

    public static bool LemmingCanDehoist(Lemming lemming, bool alreadyMoved)
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
            (!Terrain.PixelIsSolidToLemming(orientation, currentPosition) ||
             Terrain.PixelIsSolidToLemming(orientation, nextPosition)))
            return false;

        if (Terrain.PixelIsSolidToLemming(orientation, orientation.MoveDown(nextPosition, 1)))
            return false;
        if (!Terrain.PixelIsSolidToLemming(orientation, orientation.MoveDown(currentPosition, 1)))
            return true;

        if (Terrain.PixelIsSolidToLemming(orientation, orientation.MoveDown(nextPosition, 2)))
            return false;
        if (!Terrain.PixelIsSolidToLemming(orientation, orientation.MoveDown(currentPosition, 2)))
            return true;

        if (Terrain.PixelIsSolidToLemming(orientation, orientation.MoveDown(nextPosition, 3)))
            return false;
        if (!Terrain.PixelIsSolidToLemming(orientation, orientation.MoveDown(currentPosition, 3)))
            return true;

        if (Terrain.PixelIsSolidToLemming(orientation, orientation.MoveDown(nextPosition, 4)))
            return false;
        return !Terrain.PixelIsSolidToLemming(orientation, orientation.MoveDown(currentPosition, 4));
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        if (Terrain.PixelIsSolidToLemming(lemming.Orientation, lemming.LevelPosition))
        {
            base.TransitionLemmingToAction(lemming, turnAround);
            return;
        }

        FallerAction.Instance.TransitionLemmingToAction(lemming, turnAround);
    }
}