﻿using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class DisarmerSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly DisarmerSkill Instance = new();

    private DisarmerSkill()
        : base(
            LevelConstants.DisarmerSkillId,
            LevelConstants.DisarmerSkillName)
    {
    }

    public int LemmingStateChangerId => LemmingStateChangerHelper.DisarmerStateChangerId;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.IsDisarmer && ActionIsAssignable(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsDisarmer = true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill();

    public void SetLemmingState(LemmingState lemmingState, bool status)
    {
        lemmingState.IsDisarmer = status;
    }

    public void ToggleLemmingState(LemmingState lemmingState)
    {
        lemmingState.IsDisarmer = !lemmingState.IsDisarmer;
    }

    public bool IsApplied(LemmingState lemmingState)
    {
        return lemmingState.IsDisarmer;
    }
}