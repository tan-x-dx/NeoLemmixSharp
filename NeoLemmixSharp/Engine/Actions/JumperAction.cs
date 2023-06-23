namespace NeoLemmixSharp.Engine.Actions;

public sealed class JumperAction : LemmingAction
{
    public const int NumberOfJumperAnimationFrames = 13;

    public static JumperAction Instance { get; } = new();

    private JumperAction()
    {
    }

    public override int ActionId => 17;
    public override string LemmingActionName => "jumper";
    public override int NumberOfAnimationFrames => NumberOfJumperAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        return false;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        if (lemming.CurrentAction == ClimberAction.Instance ||
            lemming.CurrentAction == SliderAction.Instance)
        {
            lemming.FacingDirection = lemming.FacingDirection.OppositeDirection;
            lemming.LevelPosition = lemming.Orientation.MoveRight(lemming.LevelPosition, lemming.FacingDirection.DeltaX);
        }

        base.TransitionLemmingToAction(lemming, turnAround);
    }
}