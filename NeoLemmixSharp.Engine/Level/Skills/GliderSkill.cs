﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using static NeoLemmixSharp.Engine.Level.Skills.ILemmingStateChanger;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class GliderSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly GliderSkill Instance = new();

    private GliderSkill()
        : base(
            EngineConstants.GliderSkillId,
            EngineConstants.GliderSkillName)
    {
    }

    public StateChangerType LemmingStateChangerType => StateChangerType.GliderStateChanger;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.HasSpecialFallingBehaviour && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsGlider = true;
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

    public void SetLemmingState(LemmingState lemmingState, bool status)
    {
        lemmingState.IsGlider = status;
    }

    public void ToggleLemmingState(LemmingState lemmingState)
    {
        lemmingState.IsGlider = !lemmingState.IsGlider;
    }

    public bool IsApplied(LemmingState lemmingState)
    {
        return lemmingState.IsGlider;
    }
}