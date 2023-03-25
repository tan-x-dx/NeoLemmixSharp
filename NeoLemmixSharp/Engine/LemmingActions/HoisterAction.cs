using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class HoisterAction : ILemmingAction
{
    public const int NumberOfHoisterAnimationFrames = 8;

    public static HoisterAction Instance { get; } = new();

    private HoisterAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "hoister";
    public int NumberOfAnimationFrames => NumberOfHoisterAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is HoisterAction;
    public override bool Equals(object? obj) => obj is HoisterAction;
    public override int GetHashCode() => nameof(HoisterAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
        lemming.IsStartingAction = previouslyStartingAction;
    }
}