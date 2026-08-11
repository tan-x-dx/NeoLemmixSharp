using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class ClimberSkill : LemmingSkill, ILemmingAbilityChanger
{
    public static readonly ClimberSkill Instance = new();

    private ClimberSkill()
        : base(
            LemmingSkillType.ClimberSkill,
            LemmingSkillConstants.ClimberSkillName)
    {
    }

    public LemmingAbilityType LemmingAbilityType => LemmingAbilityType.ClimberAbility;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.IsClimber && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsClimber = true;
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

    public void SetLemmingAbility(LemmingState lemmingState, bool status)
    {
        lemmingState.IsClimber = status;
    }

    public void ToggleLemmingAbility(LemmingState lemmingState)
    {
        lemmingState.IsClimber = !lemmingState.IsClimber;
    }

    public bool LemmingHasAbility(LemmingState lemmingState)
    {
        return lemmingState.IsClimber;
    }
}
