namespace NeoLemmixSharp.Engine.Engine.Actions;

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
    public override int CursorSelectionPriorityValue => 2;

    public override bool UpdateLemming(Lemming lemming)
    {
        throw new NotImplementedException();
    }
}