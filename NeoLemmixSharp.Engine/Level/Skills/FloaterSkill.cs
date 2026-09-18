using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class FloaterSkill
{
    public static ILemmingAbilityChanger LemmingAbilityChanger { get; } = new FloaterAbilityChanger();

    private sealed class FloaterAbilityChanger : ILemmingAbilityChanger
    {
        public LemmingAbilityType LemmingAbilityType => LemmingAbilityType.FloaterAbility;

        public void SetLemmingAbility(Lemming lemming, bool status)
        {
            lemming.IsFloater = status;
        }

        public void ToggleLemmingAbility(Lemming lemming)
        {
            lemming.IsFloater = !lemming.IsFloater;
        }

        public bool LemmingHasAbility(Lemming lemming)
        {
            return lemming.IsFloater;
        }
    }


    public static bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.HasSpecialFallingBehaviour && LemmingSkillType.FloaterSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType);
    }

    public static void AssignToLemming(Lemming lemming)
    {
        lemming.IsFloater = true;
    }

    public static IEnumerable<LemmingActionType> GetActionsThatCanBeAssigned() => LemmingSkill.GetActionsThatCanBeAssignedPermanentSkill();
}
