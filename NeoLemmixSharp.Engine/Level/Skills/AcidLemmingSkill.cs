using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class AcidLemmingSkill : LemmingSkill, ILemmingAbilityChanger
{
    public static readonly AcidLemmingSkill Instance = new();

    private AcidLemmingSkill()
        : base(
            LemmingSkillType.AcidLemmingSkill,
            LemmingSkillConstants.AcidLemmingSkillName)
    {
    }

    public LemmingAbilityType LemmingAbilityType => LemmingAbilityType.AcidLemmingAbility;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.HasLiquidAffinity && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.IsAcidLemming = true;
    }

    protected override LemmingActionTypeSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

    public void SetLemmingAbility(Lemming lemming, bool status)
    {
        lemming.IsAcidLemming = status;
    }

    public void ToggleLemmingAbility(Lemming lemming)
    {
        lemming.IsAcidLemming = !lemming.IsAcidLemming;
    }

    public bool LemmingHasAbility(Lemming lemming)
    {
        return lemming.IsAcidLemming;
    }
}
