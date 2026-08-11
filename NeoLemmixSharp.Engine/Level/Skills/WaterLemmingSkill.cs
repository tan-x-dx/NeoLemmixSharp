using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class WaterLemmingSkill : LemmingSkill, ILemmingAbilityChanger
{
    public static readonly WaterLemmingSkill Instance = new();

    private WaterLemmingSkill()
        : base(
            LemmingSkillType.WaterLemmingSkill,
            LemmingSkillConstants.WaterLemmingSkillName)
    {
    }

    public LemmingAbilityType LemmingAbilityType => LemmingAbilityType.WaterLemmingAbility;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.HasLiquidAffinity && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsWaterLemming = true;
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

    public void SetLemmingAbility(LemmingState lemmingState, bool status)
    {
        lemmingState.IsWaterLemming = status;
    }

    public void ToggleLemmingAbility(LemmingState lemmingState)
    {
        lemmingState.IsWaterLemming = !lemmingState.IsWaterLemming;
    }

    public bool LemmingHasAbility(LemmingState lemmingState)
    {
        return lemmingState.IsWaterLemming;
    }
}
