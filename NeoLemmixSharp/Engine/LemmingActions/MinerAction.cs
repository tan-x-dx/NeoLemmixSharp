namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class MinerAction : LemmingAction
{
    public const int NumberOfMinerAnimationFrames = 24;

    public static MinerAction Instance { get; } = new();

    private MinerAction()
    {
    }

    protected override int ActionId => 19;
    public override string LemmingActionName => "miner";
    public override int NumberOfAnimationFrames => NumberOfMinerAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        return true;
    }
}