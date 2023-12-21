using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class SplatterAction : LemmingAction
{
    public static readonly SplatterAction Instance = new();

    private SplatterAction()
    {
    }

    public override int Id => LevelConstants.SplatterActionId;
    public override string LemmingActionName => "splatter";
    public override int NumberOfAnimationFrames => LevelConstants.SplatterAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override int CursorSelectionPriorityValue => LevelConstants.NoPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            LevelScreen.LemmingManager.RemoveLemming(lemming);
        }

        return false;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -4;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 6;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 4;
}