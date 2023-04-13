namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class JumperAction : LemmingAction
{
    public const int NumberOfJumperAnimationFrames = 13;

    public static JumperAction Instance { get; } = new();

    private JumperAction()
    {
    }

    protected override int ActionId => 17;
    public override string LemmingActionName => "jumper";
    public override int NumberOfAnimationFrames => NumberOfJumperAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        return false;
    }

    public override void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}