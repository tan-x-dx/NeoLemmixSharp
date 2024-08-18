﻿using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class WaterLemmingSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly WaterLemmingSkill Instance = new();

    private WaterLemmingSkill()
        : base(
            LevelConstants.WaterLemmingSkillId,
            LevelConstants.WaterLemmingSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.HasLiquidAffinity &&
               ActionIsAssignable(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsWaterLemming = true;
        if (lemming.CurrentAction == DrownerAction.Instance)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
        }
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        return ActionsThatCanBeAssignedPermanentSkill();
    }

    public int LemmingStateChangerId => LemmingStateChangerHelpers.WaterStateChangerId;
    public void SetLemmingState(LemmingState lemmingState, bool status)
    {
        lemmingState.IsWaterLemming = status;
    }

    public void ToggleLemmingState(LemmingState lemmingState)
    {
        lemmingState.IsWaterLemming = !lemmingState.IsWaterLemming;
    }

    public bool IsApplied(LemmingState lemmingState)
    {
        return lemmingState.IsWaterLemming;
    }
}