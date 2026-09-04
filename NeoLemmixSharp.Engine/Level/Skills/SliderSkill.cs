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
        return !lemming.IsSlider && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.IsSlider = true;
    }

    protected override LemmingActionTypeSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

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
