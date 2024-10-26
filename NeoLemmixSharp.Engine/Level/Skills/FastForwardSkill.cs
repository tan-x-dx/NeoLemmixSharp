using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class FastForwardSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly FastForwardSkill Instance = new();

    private FastForwardSkill()
        : base(
            LevelConstants.FastForwardSkillId,
            LevelConstants.FastForwardSkillName)
    {
    }

    public int LemmingStateChangerId => LemmingStateChangerHelper.FastForwardStateChangerId;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.IsPermanentFastForwards && ActionIsAssignable(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsPermanentFastForwards = true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned() => ExtendedEnumTypeComparer<LemmingAction>.CreateSimpleSet(true);

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