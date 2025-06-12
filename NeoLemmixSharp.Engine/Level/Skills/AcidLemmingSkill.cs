using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using static NeoLemmixSharp.Engine.Level.Skills.ILemmingState;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class AcidLemmingSkill : LemmingSkill, ILemmingState
{
    public static readonly AcidLemmingSkill Instance = new();

    private AcidLemmingSkill()
        : base(
            LemmingSkillConstants.AcidLemmingSkillId,
            LemmingSkillConstants.AcidLemmingSkillName)
    {
    }

    public StateType LemmingStateType => StateType.AcidLemmingState;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.HasLiquidAffinity && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsAcidLemming = true;
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

    public void SetLemmingState(LemmingState lemmingState, bool status)
    {
        lemmingState.IsAcidLemming = status;
    }

    public void ToggleLemmingState(LemmingState lemmingState)
    {
        lemmingState.IsAcidLemming = !lemmingState.IsAcidLemming;
    }

    public bool IsApplied(LemmingState lemmingState)
    {
        return lemmingState.IsAcidLemming;
    }
}