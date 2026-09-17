using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class WaterLemmingSkill
{
    public static ILemmingAbilityChanger LemmingAbilityChanger { get; } = new WaterLemmingAbilityChanger();

    private sealed class WaterLemmingAbilityChanger : ILemmingAbilityChanger
    {
        public LemmingAbilityType LemmingAbilityType => LemmingAbilityType.WaterLemmingAbility;

        public void SetLemmingAbility(Lemming lemming, bool status)
        {
            lemming.IsWaterLemming = status;
        }

        public void ToggleLemmingAbility(Lemming lemming)
        {
            lemming.IsWaterLemming = !lemming.IsWaterLemming;
        }

        public bool LemmingHasAbility(Lemming lemming)
        {
            return lemming.IsWaterLemming;
        }
    }

    public static bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.HasLiquidAffinity && LemmingSkillType.WaterLemmingSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType);
    }

    public static void AssignToLemming(Lemming lemming)
    {
        lemming.IsWaterLemming = true;
    }

    public static IEnumerable<LemmingActionType> GetActionsThatCanBeAssigned() => LemmingSkill.GetActionsThatCanBeAssignedPermanentSkill();

}
