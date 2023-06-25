namespace NeoLemmixSharp.Engine.Actions;

public sealed class ShruggerAction : LemmingAction
{
    public const int NumberOfShruggerAnimationFrames = 8;

    public static ShruggerAction Instance { get; } = new();

    private ShruggerAction()
    {
    }

    public override int Id => 24;
    public override string LemmingActionName => "shrugger";
    public override int NumberOfAnimationFrames => NumberOfShruggerAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
        }

        return true;
    }
}