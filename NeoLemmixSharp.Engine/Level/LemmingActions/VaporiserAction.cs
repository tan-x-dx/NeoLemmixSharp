using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class VaporiserAction : LemmingAction
{
    public static VaporiserAction Instance { get; } = new();

    private VaporiserAction()
    {
    }

    public override int Id => LevelConstants.VaporiserActionId;
    public override string LemmingActionName => "burner2";
    public override int NumberOfAnimationFrames => LevelConstants.VaporiserAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override int CursorSelectionPriorityValue => LevelConstants.NoPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            LevelConstants.LemmingManager.RemoveLemming(lemming);
        }

        return false;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 12;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;
    protected override int BottomRightBoundsDeltaY(int animationFrame) => 2;
}