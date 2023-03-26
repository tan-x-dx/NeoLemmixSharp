﻿using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class DisarmerAction : ILemmingAction
{
    public const int NumberOfDisarmerAnimationFrames = 16;

    public static DisarmerAction Instance { get; } = new();

    private DisarmerAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "disarmer";
    public int NumberOfAnimationFrames => NumberOfDisarmerAnimationFrames;
    public bool IsOneTimeAction => false;

    public bool Equals(ILemmingAction? other) => other is DisarmerAction;
    public override bool Equals(object? obj) => obj is DisarmerAction;
    public override int GetHashCode() => nameof(DisarmerAction).GetHashCode();

    public bool UpdateLemming(Lemming lemming)
    {
        lemming.DisarmingFrames--;
        if (lemming.DisarmingFrames <= 0)
        {
            /* ??
            if L.LemActionNew <> baNone then Transition(L, L.LemActionNew)
            else Transition(L, baWalking);
            L.LemActionNew := baNone;
            */
        }
        else if ((lemming.AnimationFrame & 7) == 0)
        {
            // ?? CueSoundEffect(SFX_FIXING, L.Position); ??
        }

        return false;
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}