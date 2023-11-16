﻿using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class HoisterAction : LemmingAction
{
    public static HoisterAction Instance { get; } = new();

    private HoisterAction()
    {
    }

    public override int Id => LevelConstants.HoisterActionId;
    public override string LemmingActionName => "hoister";
    public override int NumberOfAnimationFrames => LevelConstants.HoisterAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override int CursorSelectionPriorityValue => LevelConstants.NonWalkerMovementPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;

        if (lemming.EndOfAnimation)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        // special case due to http://www.lemmingsforums.net/index.php?topic=2620.0
        if (lemming.PhysicsFrame == 1 && lemming.IsStartingAction)
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
            return true;
        }

        if (lemming.PhysicsFrame <= 4)
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, 2);
        }

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -5;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 1;
    protected override int BottomRightBoundsDeltaY(int animationFrame) => 0;

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        var previouslyStartingAction = lemming.IsStartingAction;
        base.TransitionLemmingToAction(lemming, turnAround);
        lemming.IsStartingAction = previouslyStartingAction; // It needs to know what the Climber's value was
    }
}