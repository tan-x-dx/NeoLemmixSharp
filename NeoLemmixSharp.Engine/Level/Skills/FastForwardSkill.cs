using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using static NeoLemmixSharp.Engine.Level.Skills.ILemmingStateChanger;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class FastForwardSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly FastForwardSkill Instance = new();

    private FastForwardSkill()
        : base(
            LemmingSkillConstants.FastForwardSkillId,
            LemmingSkillConstants.FastForwardSkillName)
    {
    }

    public StateChangerType LemmingStateChangerType => StateChangerType.FastForwardStateChanger;

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