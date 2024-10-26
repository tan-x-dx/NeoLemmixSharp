using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class AcidLemmingSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly AcidLemmingSkill Instance = new();

    private AcidLemmingSkill()
        : base(
            LevelConstants.AcidLemmingSkillId,
            LevelConstants.AcidLemmingSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.HasLiquidAffinity &&
               ActionIsAssignable(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsAcidLemming = true;
        if (lemming.CurrentAction == DrownerAction.Instance)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
        }
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        return ActionsThatCanBeAssignedPermanentSkill();
    }

    public int LemmingStateChangerId => LemmingStateChangerHelper.AcidLemmingStateChangerId;
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