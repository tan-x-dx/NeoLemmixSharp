using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class SliderAction : ILemmingAction
{
    public const int NumberOfSliderAnimationFrames = 1;

    public static SliderAction Instance { get; } = new();

    private SliderAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "slider";
    public int NumberOfAnimationFrames => NumberOfSliderAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is SliderAction;
    public override bool Equals(object? obj) => obj is SliderAction;
    public override int GetHashCode() => nameof(SliderAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}