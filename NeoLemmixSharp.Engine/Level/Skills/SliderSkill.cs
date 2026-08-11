using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class SliderSkill : LemmingSkill, ILemmingAbilityChanger
{
    public static readonly SliderSkill Instance = new();

    private SliderSkill()
        : base(
            LemmingSkillType.SliderSkill,
            LemmingSkillConstants.SliderSkillName)
    {
    }

    public LemmingAbilityType LemmingAbilityType => LemmingAbilityType.SliderAbility;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.IsSlider && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsSlider = true;
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

    public void SetLemmingAbility(LemmingState lemmingState, bool status)
    {
        lemmingState.IsSlider = status;
    }

    public void ToggleLemmingAbility(LemmingState lemmingState)
    {
        lemmingState.IsSlider = !lemmingState.IsSlider;
    }

    public bool LemmingHasAbility(LemmingState lemmingState)
    {
        return lemmingState.IsSlider;
    }
}
