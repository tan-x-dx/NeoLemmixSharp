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
        return !lemming.HasLiquidAffinity && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.IsWaterLemming = true;
    }

    protected override LemmingActionTypeSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

    public void SetLemmingAbility(Lemming lemming, bool status)
    {
        lemming.IsWaterLemming = status;
    }

    public void ToggleLemmingAbility(Lemming lemming)
    {
        lemming.IsWaterLemming = !lemming.IsWaterLemming;
    }

    public bool LemmingHasAbility(Lemming lemming)
    {
        return lemming.IsWaterLemming;
    }
}
