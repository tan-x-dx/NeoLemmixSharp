namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class ShruggerAction : LemmingAction
{
    public const int NumberOfShruggerAnimationFrames = 8;

    public static ShruggerAction Instance { get; } = new();

    private ShruggerAction()
    {
    }

    protected override int ActionId => 24;
    public override string LemmingActionName => "shrugger";
    public override int NumberOfAnimationFrames => NumberOfShruggerAnimationFrames;
    public override bool IsOneTimeAction => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
        }

        return true;
    }
}