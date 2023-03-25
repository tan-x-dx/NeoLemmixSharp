using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class GliderAction : ILemmingAction
{
    public const int NumberOfGliderAnimationFrames = 17;

    public static GliderAction Instance { get; } = new();

    private GliderAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "glider";
    public int NumberOfAnimationFrames => NumberOfGliderAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is GliderAction;
    public override bool Equals(object? obj) => obj is GliderAction;
    public override int GetHashCode() => nameof(GliderAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}