namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class FencerAction : LemmingAction
{
    public const int NumberOfFencerAnimationFrames = 16;

    public static FencerAction Instance { get; } = new();

    private FencerAction()
    {
    }

    protected override int ActionId => 13;
    public override string LemmingActionName => "fencer";
    public override int NumberOfAnimationFrames => NumberOfFencerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        return false;
    }
}