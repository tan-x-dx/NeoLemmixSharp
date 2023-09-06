using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

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
    public override int CursorSelectionPriorityValue => GameConstants.NoPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            // ?? RemoveLemming(L, RM_KILL); ??
        }

        return false;
    }

    protected override int TopLeftBoundsDeltaX() => 3;
    protected override int TopLeftBoundsDeltaY() => 6;

    protected override int BottomRightBoundsDeltaX() => 4;
}