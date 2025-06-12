using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using static NeoLemmixSharp.Engine.Level.Skills.ILemmingState;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class FastForwardSkill : LemmingSkill, ILemmingState
{
    public static readonly FastForwardSkill Instance = new();

    private FastForwardSkill()
        : base(
            LemmingSkillConstants.FastForwardSkillId,
            LemmingSkillConstants.FastForwardSkillName)
    {
    }

    public StateType LemmingStateType => StateType.FastForwardState;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.IsPermanentFastForwards && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsPermanentFastForwards = true;
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

    public void SetLemmingState(LemmingState lemmingState, bool status)
    {
        lemmingState.IsPermanentFastForwards = status;
    }

    public void ToggleLemmingState(LemmingState lemmingState)
    {
        lemmingState.IsPermanentFastForwards = !lemmingState.IsPermanentFastForwards;
    }

    public bool IsApplied(LemmingState lemmingState)
    {
        return lemmingState.IsPermanentFastForwards;
    }
}