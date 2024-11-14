using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class ClimberSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly ClimberSkill Instance = new();

    private ClimberSkill()
        : base(
            EngineConstants.ClimberSkillId,
            EngineConstants.ClimberSkillName)
    {
    }

    public int LemmingStateChangerId => LemmingStateChangerHelper.ClimberStateChangerId;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.IsClimber && ActionIsAssignable(lemming);
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