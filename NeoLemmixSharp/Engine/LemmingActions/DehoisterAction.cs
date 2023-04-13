namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class DehoisterAction : LemmingAction
{
    public const int NumberOfDehoisterAnimationFrames = 7;

    public static DehoisterAction Instance { get; } = new();

    private DehoisterAction()
    {
    }

    public override string LemmingActionName => "dehoister";
    public override int NumberOfAnimationFrames => NumberOfDehoisterAnimationFrames;
    public override bool IsOneTimeAction => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        return true;
    }

    public override void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}