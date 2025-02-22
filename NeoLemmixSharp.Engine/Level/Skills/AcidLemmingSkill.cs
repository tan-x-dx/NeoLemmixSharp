using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using static NeoLemmixSharp.Engine.Level.Skills.ILemmingStateChanger;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class AcidLemmingSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly AcidLemmingSkill Instance = new();

    private AcidLemmingSkill()
        : base(
            EngineConstants.AcidLemmingSkillId,
            EngineConstants.AcidLemmingSkillName)
    {
    }

    public StateChangerType LemmingStateChangerType => StateChangerType.AcidLemmingStateChanger;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.HasLiquidAffinity && ActionIsAssignable(lemming);
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