using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class GliderSkill
{
    public static ILemmingAbilityChanger LemmingAbilityChanger { get; } = new GliderAbilityChanger();

    private sealed class GliderAbilityChanger : ILemmingAbilityChanger
    {
        public LemmingAbilityType LemmingAbilityType => LemmingAbilityType.GliderAbility;

        public void SetLemmingAbility(Lemming lemming, bool status)
        {
            lemming.IsGlider = status;
        }

        public void ToggleLemmingAbility(Lemming lemming)
        {
            lemming.IsGlider = !lemming.IsGlider;
        }

        public bool LemmingHasAbility(Lemming lemming)
        {
            return lemming.IsGlider;
        }
    }

    public static bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.HasSpecialFallingBehaviour && LemmingSkillType.GliderSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType);
    }

    public static void AssignToLemming(Lemming lemming)
    {
        lemming.IsGlider = true;
    }

    public static IEnumerable<LemmingActionType> GetActionsThatCanBeAssigned() => LemmingSkill.GetActionsThatCanBeAssignedPermanentSkill();
}
