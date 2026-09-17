using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class ClimberSkill
{
    public static ILemmingAbilityChanger LemmingAbilityChanger { get; } = new ClimberAbilityChanger();

    private sealed class ClimberAbilityChanger : ILemmingAbilityChanger
    {
        public LemmingAbilityType LemmingAbilityType => LemmingAbilityType.ClimberAbility;

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

    public static bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.IsClimber && LemmingSkillType.ClimberSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType);
    }

    public static void AssignToLemming(Lemming lemming)
    {
        lemming.IsClimber = true;
    }

    public static IEnumerable<LemmingActionType> GetActionsThatCanBeAssigned() => LemmingSkill.GetActionsThatCanBeAssignedPermanentSkill();
}
