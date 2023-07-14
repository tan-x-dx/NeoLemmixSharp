namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class MinerAction : LemmingAction
{
    public const int NumberOfMinerAnimationFrames = 24;

    public static MinerAction Instance { get; } = new();

    private MinerAction()
    {
    }

    public override int Id => 6;
    public override string LemmingActionName => "miner";
    public override int NumberOfAnimationFrames => NumberOfMinerAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        return true;
    }
}