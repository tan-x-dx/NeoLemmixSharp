namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class StonerAction : LemmingAction
{
    public const int NumberOfStonerAnimationFrames = 16;

    public static StonerAction Instance { get; } = new();

    private StonerAction()
    {
    }

    public override int Id => 28;
    public override string LemmingActionName => "stoner";
    public override int NumberOfAnimationFrames => NumberOfStonerAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override bool CanBeAssignedPermanentSkill => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        return true;
    }
}