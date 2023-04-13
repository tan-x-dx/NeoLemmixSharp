namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class ShruggerAction : LemmingAction
{
    public const int NumberOfShruggerAnimationFrames = 8;

    public static ShruggerAction Instance { get; } = new();

    private ShruggerAction()
    {
    }

    public override string LemmingActionName => "shrugger";
    public override int NumberOfAnimationFrames => NumberOfShruggerAnimationFrames;
    public override bool IsOneTimeAction => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            CommonMethods.TransitionToNewAction(lemming, WalkerAction.Instance, false);
        }

        return true;
    }

    public override void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}