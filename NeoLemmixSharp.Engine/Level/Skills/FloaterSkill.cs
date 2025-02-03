using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class FloaterSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly FloaterSkill Instance = new();

    private FloaterSkill()
        : base(
            EngineConstants.FloaterSkillId,
            EngineConstants.FloaterSkillName)
    {
    }

    public int LemmingStateChangerId => LemmingStateChangerHasher.FloaterStateChangerId;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.HasSpecialFallingBehaviour && ActionIsAssignable(lemming);
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