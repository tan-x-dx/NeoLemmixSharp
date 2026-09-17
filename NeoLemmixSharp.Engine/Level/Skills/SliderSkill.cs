using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class SliderSkill
{
    public static ILemmingAbilityChanger LemmingAbilityChanger { get; } = new SliderAbilityChanger();

    private sealed class SliderAbilityChanger : ILemmingAbilityChanger
    {
        public LemmingAbilityType LemmingAbilityType => LemmingAbilityType.SliderAbility;

        public void SetLemmingAbility(Lemming lemming, bool status)
        {
            lemming.IsSlider = status;
        }

        public void ToggleLemmingAbility(Lemming lemming)
        {
            lemming.IsSlider = !lemming.IsSlider;
        }

        public bool LemmingHasAbility(Lemming lemming)
        {
            return lemming.IsSlider;
        }
    }

    public static bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.IsSlider && LemmingSkillType.BuilderSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType);
    }

    public static void AssignToLemming(Lemming lemming)
    {
        lemming.IsSlider = true;
    }

    public static IEnumerable<LemmingActionType> GetActionsThatCanBeAssigned() => LemmingSkill.GetActionsThatCanBeAssignedPermanentSkill();
}
