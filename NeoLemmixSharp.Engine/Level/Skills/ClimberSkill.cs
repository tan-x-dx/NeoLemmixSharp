using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using static NeoLemmixSharp.Engine.Level.Skills.ILemmingState;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class ClimberSkill : LemmingSkill, ILemmingState
{
    public static readonly ClimberSkill Instance = new();

    private ClimberSkill()
        : base(
            LemmingSkillConstants.ClimberSkillId,
            LemmingSkillConstants.ClimberSkillName)
    {
    }

    public StateType LemmingStateType => StateType.ClimberState;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.IsClimber && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsClimber = true;
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

    public void SetLemmingState(LemmingState lemmingState, bool status)
    {
        lemmingState.IsClimber = status;
    }

    public void ToggleLemmingState(LemmingState lemmingState)
    {
        lemmingState.IsClimber = !lemmingState.IsClimber;
    }

    public bool IsApplied(LemmingState lemmingState)
    {
        return lemmingState.IsClimber;
    }
}
