using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class DisarmerSkill : LemmingSkill, ILemmingState
{
    public static readonly DisarmerSkill Instance = new();

    private DisarmerSkill()
        : base(
            LemmingSkillType.DisarmerSkill,
            LemmingSkillConstants.DisarmerSkillName)
    {
    }

    public StateType LemmingStateType => StateType.DisarmerState;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.IsDisarmer && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsDisarmer = true;
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

    public void SetLemmingState(LemmingState lemmingState, bool status)
    {
        lemmingState.IsDisarmer = status;
    }

    public void ToggleLemmingState(LemmingState lemmingState)
    {
        lemmingState.IsDisarmer = !lemmingState.IsDisarmer;
    }

    public bool IsApplied(LemmingState lemmingState)
    {
        return lemmingState.IsDisarmer;
    }
}
