﻿using NeoLemmixSharp.Engine.Engine.Actions;
using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class ClimberSkill : LemmingSkill
{
    public static ClimberSkill Instance { get; } = new();

    private ClimberSkill()
    {
    }

    public override int Id => GameConstants.ClimberSkillId;
    public override string LemmingSkillName => "climber";
    public override bool IsPermanentSkill => true;
    public override bool IsClassicSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.IsClimber && ActionIsAssignable(lemming);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        lemming.State.IsClimber = true;
        return true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill();
}