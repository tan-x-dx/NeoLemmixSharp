namespace NeoLemmixSharp.Engine.Actions;

public sealed class WalkerAction : LemmingAction
{
    public const int NumberOfWalkerAnimationFrames = 8;

    public static WalkerAction Instance { get; } = new();

    private WalkerAction()
    {
    }

    public override int Id => 31;
    public override string LemmingActionName => "walker";
    public override int NumberOfAnimationFrames => NumberOfWalkerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        var dx = lemming.FacingDirection.DeltaX;
        var lemmingPosition = lemming.LevelPosition;
        lemmingPosition = lemming.Orientation.MoveRight(lemmingPosition, dx);
        lemming.LevelPosition = lemmingPosition;
        var dy = FindGroundPixel(lemming, lemming.Orientation, lemmingPosition);

        if (dy > 0 &&
            lemming.IsSlider &&
            LemmingCanDehoist(lemming, true))
        {
            lemmingPosition = lemming.Orientation.MoveLeft(lemmingPosition, dx);
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
                lemming.FacingDirection = lemming.FacingDirection.OppositeDirection;
                lemmingPosition = lemming.Orientation.MoveLeft(lemmingPosition, dx);
                lemming.LevelPosition = lemmingPosition;
            }
        }
        else if (dy < -2)
        {
            AscenderAction.Instance.TransitionLemmingToAction(lemming, false);
            lemmingPosition = lemming.Orientation.MoveUp(lemmingPosition, 2);
            lemming.LevelPosition = lemmingPosition;
        }
        else if (dy < 1)
        {
            lemmingPosition = lemming.Orientation.MoveDown(lemmingPosition, dy);
            lemming.LevelPosition = lemmingPosition;
        }

        // Get new ground pixel again in case the Lem has turned
        dy = FindGroundPixel(lemming, lemming.Orientation, lemmingPosition);

        if (dy > 3)
        {
            lemmingPosition = lemming.Orientation.MoveDown(lemmingPosition, 4);
            lemming.LevelPosition = lemmingPosition;
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
        }
        else if (dy > 0)
        {
            lemmingPosition = lemming.Orientation.MoveDown(lemmingPosition, dy);
            lemming.LevelPosition = lemmingPosition;
        }

        return true;
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