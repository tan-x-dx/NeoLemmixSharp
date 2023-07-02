namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class SwimmerAction : LemmingAction
{
    public const int NumberOfSwimmerAnimationFrames = 8;

    public static SwimmerAction Instance { get; } = new();

    private SwimmerAction()
    {
    }

    public override int Id => 29;
    public override string LemmingActionName => "swimmer";
    public override int NumberOfAnimationFrames => NumberOfSwimmerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        return true;
    }
}