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
        return !lemming.State.HasLiquidAffinity && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsAcidLemming = true;
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

    public void SetLemmingAbility(LemmingState lemmingState, bool status)
    {
        lemmingState.IsAcidLemming = status;
    }

    public void ToggleLemmingAbility(LemmingState lemmingState)
    {
        lemmingState.IsAcidLemming = !lemmingState.IsAcidLemming;
    }

    public bool LemmingHasAbility(LemmingState lemmingState)
    {
        return lemmingState.IsAcidLemming;
    }
}
