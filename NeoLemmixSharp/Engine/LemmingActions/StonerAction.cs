namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class StonerAction : LemmingAction
{
    public const int NumberOfStonerAnimationFrames = 16;

    public static StonerAction Instance { get; } = new();

    private StonerAction()
    {
    }

    protected override int ActionId => 28;
    public override string LemmingActionName => "stoner";
    public override int NumberOfAnimationFrames => NumberOfStonerAnimationFrames;
    public override bool IsOneTimeAction => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        return true;
    }
}