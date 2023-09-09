using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class JumperAction : LemmingAction
{
    public static JumperAction Instance { get; } = new();

    private JumperAction()
    {
    }

    public override int Id => GameConstants.JumperActionId;
    public override string LemmingActionName => "jumper";
    public override int NumberOfAnimationFrames => GameConstants.JumperAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => GameConstants.NonWalkerMovementPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        throw new NotImplementedException();
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -1;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 9;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        if (lemming.CurrentAction == ClimberAction.Instance ||
            lemming.CurrentAction == SliderAction.Instance)
        {
            lemming.SetFacingDirection(lemming.FacingDirection.OppositeDirection());
            lemming.LevelPosition = lemming.Orientation.MoveRight(lemming.LevelPosition, lemming.FacingDirection.DeltaX);
        }

        base.TransitionLemmingToAction(lemming, turnAround);
    }
}