﻿namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class ShruggerAction : LemmingAction
{
    public static ShruggerAction Instance { get; } = new();

    private ShruggerAction()
    {
    }

    public override int Id => 19;
    public override string LemmingActionName => "shrugger";
    public override int NumberOfAnimationFrames => GameConstants.ShruggerAnimationFrames;
    public override bool IsOneTimeAction => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
        }

        return true;
    }
}