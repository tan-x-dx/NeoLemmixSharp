namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class ExploderAction : LemmingAction
{
    public const int NumberOfExploderAnimationFrames = 1;

    public static ExploderAction Instance { get; } = new();

    private ExploderAction()
    {
    }

    public override int ActionId => 11;
    public override string LemmingActionName => "bomber";
    public override int NumberOfAnimationFrames => NumberOfExploderAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override bool CanBeAssignedPermanentSkill => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        return false;
    }
}