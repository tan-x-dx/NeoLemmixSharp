namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class JumperAction : LemmingAction
{
    public static JumperAction Instance { get; } = new();

    private JumperAction()
    {
    }

    public override int Id => 12;
    public override string LemmingActionName => "jumper";
    public override int NumberOfAnimationFrames => GameConstants.JumperAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        return false;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        if (lemming.CurrentAction == ClimberAction.Instance ||
            lemming.CurrentAction == SliderAction.Instance)
        {
            lemming.SetFacingDirection(lemming.FacingDirection.OppositeDirection);
            lemming.LevelPosition = lemming.Orientation.MoveRight(lemming.LevelPosition, lemming.FacingDirection.DeltaX);
        }

        base.TransitionLemmingToAction(lemming, turnAround);
    }
}