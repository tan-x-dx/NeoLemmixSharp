﻿namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class HoisterAction : LemmingAction
{
    public const int NumberOfHoisterAnimationFrames = 8;

    public static HoisterAction Instance { get; } = new();

    private HoisterAction()
    {
    }

    public override int Id => 21;
    public override string LemmingActionName => "hoister";
    public override int NumberOfAnimationFrames => NumberOfHoisterAnimationFrames;
    public override bool IsOneTimeAction => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        // special case due to http://www.lemmingsforums.net/index.php?topic=2620.0
        if (lemming.AnimationFrame == 1 && lemming.IsStartingAction)
        {
            lemming.LevelPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, 1);
            return true;
        }

        if (lemming.AnimationFrame <= 4)
        {
            lemming.LevelPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, 2);
        }

        return true;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        var previouslyStartingAction = lemming.IsStartingAction;
        base.TransitionLemmingToAction(lemming, turnAround);
        lemming.IsStartingAction = previouslyStartingAction;
    }
}