namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class DehoisterAction : LemmingAction
{
    public const int NumberOfDehoisterAnimationFrames = 7;

    public static DehoisterAction Instance { get; } = new();

    private DehoisterAction()
    {
    }

    public override int Id => 22;
    public override string LemmingActionName => "dehoister";
    public override int NumberOfAnimationFrames => NumberOfDehoisterAnimationFrames;
    public override bool IsOneTimeAction => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;
        var dx = lemming.FacingDirection.DeltaX;

        if (lemming.EndOfAnimation)
        {
            if (Terrain.PixelIsSolidToLemming(orientation, orientation.MoveUp(lemmingPosition, 7)))
            {
                SliderAction.Instance.TransitionLemmingToAction(lemming, false);
                return true;
            }

            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        if (lemming.AnimationFrame < 2)
            return true;

        lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
        lemming.LevelPosition = lemmingPosition;

        var animFrameValue = lemming.AnimationFrame * 2;

        if (!SliderAction.SliderTerrainChecks(lemming, orientation, animFrameValue - 3))
        {
            if (lemming.CurrentAction == DrownerAction.Instance)
                return false;
        }

        lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
        lemming.LevelPosition = lemmingPosition;

        if (SliderAction.SliderTerrainChecks(lemming, orientation, animFrameValue - 2))
            return true;

        return lemming.CurrentAction != DrownerAction.Instance;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        lemming.DehoistPin = lemming.LevelPosition;

        base.TransitionLemmingToAction(lemming, turnAround);
    }
}