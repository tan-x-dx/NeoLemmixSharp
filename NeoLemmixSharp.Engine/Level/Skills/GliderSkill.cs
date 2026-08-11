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
        return !lemming.State.HasSpecialFallingBehaviour && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsGlider = true;
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

    public void SetLemmingAbility(LemmingState lemmingState, bool status)
    {
        lemmingState.IsGlider = status;
    }

    public void ToggleLemmingAbility(LemmingState lemmingState)
    {
        lemmingState.IsGlider = !lemmingState.IsGlider;
    }

    public bool LemmingHasAbility(LemmingState lemmingState)
    {
        return lemmingState.IsGlider;
    }
}
