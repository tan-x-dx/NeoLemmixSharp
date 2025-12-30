using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using static NeoLemmixSharp.Engine.Level.Skills.ILemmingState;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class WaterLemmingSkill : LemmingSkill, ILemmingState
{
    public static readonly WaterLemmingSkill Instance = new();

    private WaterLemmingSkill()
        : base(
            LemmingSkillConstants.WaterLemmingSkillId,
            LemmingSkillConstants.WaterLemmingSkillName)
    {
    }

    public StateType LemmingStateType => StateType.WaterState;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.HasLiquidAffinity && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsWaterLemming = true;
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

    public void SetLemmingState(LemmingState lemmingState, bool status)
    {
        lemmingState.IsWaterLemming = status;
    }

    public void ToggleLemmingState(LemmingState lemmingState)
    {
        lemmingState.IsWaterLemming = !lemmingState.IsWaterLemming;
    }

    public bool IsApplied(LemmingState lemmingState)
    {
        return lemmingState.IsWaterLemming;
    }
}
