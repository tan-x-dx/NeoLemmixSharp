using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class AcidLemmingSkill
{
    public static ILemmingAbilityChanger LemmingAbilityChanger { get; } = new AcidLemmingAbilityChanger();

    private sealed class AcidLemmingAbilityChanger : ILemmingAbilityChanger
    {
        public LemmingAbilityType LemmingAbilityType => LemmingAbilityType.AcidLemmingAbility;

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

    public static bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.HasLiquidAffinity && LemmingSkillType.AcidLemmingSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType);
    }

    public static void AssignToLemming(Lemming lemming)
    {
        lemming.IsAcidLemming = true;
    }

    public static IEnumerable<LemmingActionType> GetActionsThatCanBeAssigned() => LemmingSkill.GetActionsThatCanBeAssignedPermanentSkill();
}
