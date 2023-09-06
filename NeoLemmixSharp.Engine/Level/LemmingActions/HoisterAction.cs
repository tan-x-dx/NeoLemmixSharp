using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class HoisterAction : LemmingAction
{
    public static HoisterAction Instance { get; } = new();

    private HoisterAction()
    {
    }

    public override int Id => GameConstants.HoisterActionId;
    public override string LemmingActionName => "hoister";
    public override int NumberOfAnimationFrames => GameConstants.HoisterAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override int CursorSelectionPriorityValue => GameConstants.NonWalkerMovementPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;

        if (lemming.EndOfAnimation)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        // special case due to http://www.lemmingsforums.net/index.php?topic=2620.0
        if (lemming.AnimationFrame == 1 && lemming.IsStartingAction)
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
            lemming.LevelPosition = lemmingPosition;
            return true;
        }

        if (lemming.AnimationFrame <= 4)
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, 2);
            lemming.LevelPosition = lemmingPosition;
        }

        return true;
    }

    public override LevelPosition GetAnchorPosition() => new(5, 12);

    protected override int TopLeftBoundsDeltaX() => -4;
    protected override int TopLeftBoundsDeltaY() => 11;

    protected override int BottomRightBoundsDeltaX() => 0;

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        var previouslyStartingAction = lemming.IsStartingAction;
        base.TransitionLemmingToAction(lemming, turnAround);
        lemming.IsStartingAction = previouslyStartingAction;
    }
}