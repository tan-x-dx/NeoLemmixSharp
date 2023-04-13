namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class DehoisterAction : LemmingAction
{
    public const int NumberOfDehoisterAnimationFrames = 7;

    public static DehoisterAction Instance { get; } = new();

    private DehoisterAction()
    {
    }

    protected override int ActionId => 6;
    public override string LemmingActionName => "dehoister";
    public override int NumberOfAnimationFrames => NumberOfDehoisterAnimationFrames;
    public override bool IsOneTimeAction => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        return true;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        lemming.DehoistPin = lemming.LevelPosition;

        base.TransitionLemmingToAction(lemming, turnAround);
    }
}