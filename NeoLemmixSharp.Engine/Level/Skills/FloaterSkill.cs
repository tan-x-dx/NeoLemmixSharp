using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class FloaterSkill : LemmingSkill, ILemmingState
{
    public static readonly FloaterSkill Instance = new();

    private FloaterSkill()
        : base(
            LemmingSkillType.FloaterSkill,
            LemmingSkillConstants.FloaterSkillName)
    {
    }

    public StateType LemmingStateType => StateType.FloaterState;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.HasSpecialFallingBehaviour && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsFloater = true;
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

    public void SetLemmingState(LemmingState lemmingState, bool status)
    {
        lemmingState.IsFloater = status;
    }

    public void ToggleLemmingState(LemmingState lemmingState)
    {
        lemmingState.IsFloater = !lemmingState.IsFloater;
    }

    public bool IsApplied(LemmingState lemmingState)
    {
        return lemmingState.IsFloater;
    }
}
