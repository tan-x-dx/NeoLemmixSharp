using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class FastForwardSkill
{
    public static ILemmingAbilityChanger LemmingAbilityChanger { get; } = new FastForwardAbilityChanger();

    private sealed class FastForwardAbilityChanger : ILemmingAbilityChanger
    {
        public LemmingAbilityType LemmingAbilityType => LemmingAbilityType.FastForwardAbility;

        public void SetLemmingAbility(Lemming lemming, bool status)
        {
            lemming.IsPermanentFastForwards = status;
        }

        public void ToggleLemmingAbility(Lemming lemming)
        {
            lemming.IsPermanentFastForwards = !lemming.IsPermanentFastForwards;
        }

        public bool LemmingHasAbility(Lemming lemming)
        {
            return lemming.IsPermanentFastForwards;
        }
    }

    public static bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.IsPermanentFastForwards && LemmingSkillType.FastForwardSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType);
    }

    public static void AssignToLemming(Lemming lemming)
    {
        lemming.IsPermanentFastForwards = true;
    }

    public static IEnumerable<LemmingActionType> GetActionsThatCanBeAssigned() => LemmingSkill.GetActionsThatCanBeAssignedPermanentSkill();
}
