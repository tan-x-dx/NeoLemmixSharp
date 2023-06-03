﻿namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class VaporiserAction : LemmingAction
{
    public const int NumberOfVaporiserAnimationFrames = 16;

    public static VaporiserAction Instance { get; } = new();

    private VaporiserAction()
    {
    }

    public override int ActionId => 30;
    public override string LemmingActionName => "burner";
    public override int NumberOfAnimationFrames => NumberOfVaporiserAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override bool CanBeAssignedPermanentSkill => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            // ?? RemoveLemming(L, RM_KILL); ??
        }

        return false;
    }
}