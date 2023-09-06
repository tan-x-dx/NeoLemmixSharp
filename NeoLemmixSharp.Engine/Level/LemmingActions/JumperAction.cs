using NeoLemmixSharp.Common.Util;
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

    public override LevelPosition GetAnchorPosition() => new(2, 10);

    protected override int TopLeftBoundsDeltaX() => -1;
    protected override int TopLeftBoundsDeltaY() => 9;

    protected override int BottomRightBoundsDeltaX() => 3;

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