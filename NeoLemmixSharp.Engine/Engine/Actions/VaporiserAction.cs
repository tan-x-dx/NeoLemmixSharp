namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class VaporiserAction : LemmingAction
{
    public const int NumberOfVaporiserAnimationFrames = 16;

    public static VaporiserAction Instance { get; } = new();

    private VaporiserAction()
    {
    }

    public override int Id => 30;
    public override string LemmingActionName => "burner2";
    public override int NumberOfAnimationFrames => NumberOfVaporiserAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override bool CanBeAssignedPermanentSkill => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            // ?? RemoveLemming(L, RM_KILL); ??
        }

        return false;
    }
}