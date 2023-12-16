using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class ShruggerAction : LemmingAction
{
    public static readonly ShruggerAction Instance = new();

    private ShruggerAction()
    {
    }

    public override int Id => LevelConstants.ShruggerActionId;
    public override string LemmingActionName => "shrugger";
    public override int NumberOfAnimationFrames => LevelConstants.ShruggerAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override int CursorSelectionPriorityValue => LevelConstants.NonWalkerMovementPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
        }

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;
}