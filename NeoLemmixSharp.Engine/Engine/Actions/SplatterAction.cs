namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class SplatterAction : LemmingAction
{
    public static SplatterAction Instance { get; } = new();

    private SplatterAction()
    {
    }

    public override int Id => GameConstants.SplatterActionId;
    public override string LemmingActionName => "splatter";
    public override int NumberOfAnimationFrames => GameConstants.SplatterAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override int CursorSelectionPriorityValue => 2;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            // ?? RemoveLemming(L, RM_KILL); ??
        }

        return false;
    }
}