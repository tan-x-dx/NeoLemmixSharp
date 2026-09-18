using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class DisarmerSkill
{
    public static ILemmingAbilityChanger LemmingAbilityChanger { get; } = new DisarmerAbilityChanger();

    private sealed class DisarmerAbilityChanger : ILemmingAbilityChanger
    {
        public LemmingAbilityType LemmingAbilityType => LemmingAbilityType.DisarmerAbility;

        public void SetLemmingAbility(Lemming lemming, bool status)
        {
            lemming.IsDisarmer = status;
        }

        public void ToggleLemmingAbility(Lemming lemming)
        {
            lemming.IsDisarmer = !lemming.IsDisarmer;
        }

        public bool LemmingHasAbility(Lemming lemming)
        {
            return lemming.IsDisarmer;
        }
    }

    public static bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.IsDisarmer && LemmingSkillType.DisarmerSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType);
    }

    public static void AssignToLemming(Lemming lemming)
    {
        lemming.IsDisarmer = true;
    }

    public static IEnumerable<LemmingActionType> GetActionsThatCanBeAssigned() => LemmingSkill.GetActionsThatCanBeAssignedPermanentSkill();
}
