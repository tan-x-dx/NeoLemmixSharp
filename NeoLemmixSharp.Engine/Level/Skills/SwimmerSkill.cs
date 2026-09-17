using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class SwimmerSkill
{
    public static ILemmingAbilityChanger LemmingAbilityChanger { get; } = new SwimmerAbilityChanger();

    private sealed class SwimmerAbilityChanger : ILemmingAbilityChanger
    {
        public LemmingAbilityType LemmingAbilityType => LemmingAbilityType.SwimmerAbility;

        public void SetLemmingAbility(Lemming lemming, bool status)
        {
            lemming.IsSwimmer = status;
        }

        public void ToggleLemmingAbility(Lemming lemming)
        {
            lemming.IsSwimmer = !lemming.IsSwimmer;
        }

        public bool LemmingHasAbility(Lemming lemming)
        {
            return lemming.IsSwimmer;
        }
    }

    public static bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.HasLiquidAffinity && LemmingSkillType.SwimmerSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType);
    }

    public static void AssignToLemming(Lemming lemming)
    {
        lemming.IsSwimmer = true;
        if (lemming.CurrentActionType == LemmingActionType.DrownerAction)
        {
            SwimmerAction.TransitionLemmingToAction(lemming, false);
        }
    }

    public static IEnumerable<LemmingActionType> GetActionsThatCanBeAssigned() => LemmingSkill.GetActionsThatCanBeAssignedPermanentSkill().Append(LemmingActionType.DrownerAction);
}
