namespace NeoLemmixSharp.Engine.Actions;

public sealed class SplatterAction : LemmingAction
{
    public const int NumberOfSplatterAnimationFrames = 16;

    public static SplatterAction Instance { get; } = new();

    private SplatterAction()
    {
    }

    public override int Id => 26;
    public override string LemmingActionName => "splatter";
    public override int NumberOfAnimationFrames => NumberOfSplatterAnimationFrames;
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