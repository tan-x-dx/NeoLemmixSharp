namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class ExploderAction : LemmingAction
{
    public const int NumberOfExploderAnimationFrames = 1;

    public static ExploderAction Instance { get; } = new();

    private ExploderAction()
    {
    }

    public override string LemmingActionName => "bomber";
    public override int NumberOfAnimationFrames => NumberOfExploderAnimationFrames;
    public override bool IsOneTimeAction => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        return false;
    }

    public override void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}