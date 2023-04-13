namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class SwimmerAction : LemmingAction
{
    public const int NumberOfSwimmerAnimationFrames = 8;

    public static SwimmerAction Instance { get; } = new();

    private SwimmerAction()
    {
    }

    protected override int ActionId => 29;
    public override string LemmingActionName => "swimmer";
    public override int NumberOfAnimationFrames => NumberOfSwimmerAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        return true;
    }

    public override void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}