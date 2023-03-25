using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class MinerAction : ILemmingAction
{
    public const int NumberOfMinerAnimationFrames = 24;

    public static MinerAction Instance { get; } = new();

    private MinerAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "miner";
    public int NumberOfAnimationFrames => NumberOfMinerAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is MinerAction;
    public override bool Equals(object? obj) => obj is MinerAction;
    public override int GetHashCode() => nameof(MinerAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}