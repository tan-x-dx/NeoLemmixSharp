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
        return !lemming.IsClimber && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.IsClimber = true;
    }

    protected override LemmingActionTypeSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

    public void SetLemmingAbility(Lemming lemming, bool status)
    {
        lemming.IsClimber = status;
    }

    public void ToggleLemmingAbility(Lemming lemming)
    {
        lemming.IsClimber = !lemming.IsClimber;
    }

    public bool LemmingHasAbility(Lemming lemming)
    {
        return lemming.IsClimber;
    }
}
