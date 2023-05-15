namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class DrownerAction : LemmingAction
{
    public const int NumberOfDrownerAnimationFrames = 16;

    public static DrownerAction Instance { get; } = new();

    private DrownerAction()
    {
    }

    protected override int ActionId => 9;
    public override string LemmingActionName => "drowner";
    public override int NumberOfAnimationFrames => NumberOfDrownerAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override bool CanBeAssignedPermanentSkill => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            // remove lemming
        }

        return false;
    }
}