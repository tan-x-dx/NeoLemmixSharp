using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class StonerAction : LemmingAction
{
    public static StonerAction Instance { get; } = new();

    private StonerAction()
    {
    }

    public override int Id => GameConstants.StonerActionId;
    public override string LemmingActionName => "stoner";
    public override int NumberOfAnimationFrames => GameConstants.StonerAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override int CursorSelectionPriorityValue => GameConstants.NoPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        throw new NotImplementedException();
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -5;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 5;
}