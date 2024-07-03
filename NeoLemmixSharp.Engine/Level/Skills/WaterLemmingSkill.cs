using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class WaterLemmingSkill : LemmingSkill
{
    public static readonly WaterLemmingSkill Instance = new();

    private WaterLemmingSkill()
        : base(
            LevelConstants.WaterLemmingSkillId,
            LevelConstants.WaterLemmingSkillName,
            false)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.HasLiquidAffinity &&
               ActionIsAssignable(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsWaterLemming = true;
        if (lemming.CurrentAction == DrownerAction.Instance)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
        }
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        return ActionsThatCanBeAssignedPermanentSkill();
    }
}