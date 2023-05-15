namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class WalkerAction : LemmingAction
{
    public const int NumberOfWalkerAnimationFrames = 8;

    public static WalkerAction Instance { get; } = new();

    private WalkerAction()
    {
    }

    protected override int ActionId => 31;
    public override string LemmingActionName => "walker";
    public override int NumberOfAnimationFrames => NumberOfWalkerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        var dx = lemming.FacingDirection.DeltaX;
        lemming.LevelPosition = lemming.Orientation.MoveRight(lemming.LevelPosition, dx);
        var dy = FindGroundPixel(lemming.Orientation, lemming.LevelPosition);

        if (dy > 0 &&
            lemming.IsSlider &&
            LemmingCanDehoist(lemming, true))
        {
            lemming.LevelPosition = lemming.Orientation.MoveLeft(lemming.LevelPosition, dx);
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
                lemming.LevelPosition = lemming.Orientation.MoveLeft(lemming.LevelPosition, dx);
            }
        }
        else if (dy < -2)
        {
            AscenderAction.Instance.TransitionLemmingToAction(lemming, false);
            lemming.LevelPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, 2);
        }
        else if (dy < 1)
        {
            lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, dy);
        }

        // Get new ground pixel again in case the Lem has turned
        dy = FindGroundPixel(lemming.Orientation, lemming.LevelPosition);

        if (dy > 3)
        {
            lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, 4);
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
        }
        else if (dy > 0)
        {
            lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, dy);
        }

        return true;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        if (Terrain.GetPixelData(lemming.LevelPosition).IsSolid)
        {
            base.TransitionLemmingToAction(lemming, turnAround);
            return;
        }

        FallerAction.Instance.TransitionLemmingToAction(lemming, turnAround);
    }
}