namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class StonerAction : LemmingAction
{
    public static StonerAction Instance { get; } = new();

    private StonerAction()
    {
    }

    public override int Id => 29;
    public override string LemmingActionName => "stoner";
    public override int NumberOfAnimationFrames => GameConstants.StonerAnimationFrames;
    public override bool IsOneTimeAction => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        return true;
    }
}