using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class ShruggerAction : ILemmingAction
{
    public const int NumberOfShruggerAnimationFrames = 8;

    public static ShruggerAction Instance { get; } = new();

    private ShruggerAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "shrugger";
    public int NumberOfAnimationFrames => NumberOfShruggerAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is ShruggerAction;
    public override bool Equals(object? obj) => obj is ShruggerAction;
    public override int GetHashCode() => nameof(ShruggerAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
        if (lemming.AnimationFrame == NumberOfAnimationFrames)
        {
            CommonMethods.TransitionToNewAction(lemming, WalkerAction.Instance, false);
        }
    }

    public void OnTransitionToAction(Lemming lemming)
    {
    }
}