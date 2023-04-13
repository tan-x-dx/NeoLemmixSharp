namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class DrownerAction : LemmingAction
{
    public const int NumberOfDrownerAnimationFrames = 16;

    public static DrownerAction Instance { get; } = new();

    private DrownerAction()
    {
    }

    public override string LemmingActionName => "drowner";
    public override int NumberOfAnimationFrames => NumberOfDrownerAnimationFrames;
    public override bool IsOneTimeAction => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            // remove lemming
        }

        return false;
    }

    public override void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}