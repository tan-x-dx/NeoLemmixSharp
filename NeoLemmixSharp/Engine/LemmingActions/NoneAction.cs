namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class NoneAction : LemmingAction
{
    /// <summary>
    /// Logically equivalent to null, but null references suck
    /// </summary>
    public static NoneAction Instance { get; } = new();

    private NoneAction()
    {
    }

    public override int ActionId => -1;
    public override string LemmingActionName => "none";
    public override int NumberOfAnimationFrames => 1;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => false;
    public override bool UpdateLemming(Lemming lemming)
    {
        return false;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
    }
}