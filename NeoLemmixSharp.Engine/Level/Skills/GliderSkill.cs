using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class GliderSkill : LemmingSkill, ILemmingAbilityChanger
{
    public static readonly GliderSkill Instance = new();

    private GliderSkill()
        : base(
            LemmingSkillType.GliderSkill,
            LemmingSkillConstants.GliderSkillName)
    {
    }

    public LemmingAbilityType LemmingAbilityType => LemmingAbilityType.GliderAbility;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.HasSpecialFallingBehaviour && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.IsGlider = true;
    }

    protected override LemmingActionTypeSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

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
