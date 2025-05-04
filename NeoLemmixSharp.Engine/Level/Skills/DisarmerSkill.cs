using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using static NeoLemmixSharp.Engine.Level.Skills.ILemmingStateChanger;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class DisarmerSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly DisarmerSkill Instance = new();

    private DisarmerSkill()
        : base(
            EngineConstants.DisarmerSkillId,
            EngineConstants.DisarmerSkillName)
    {
    }

    public StateChangerType LemmingStateChangerType => StateChangerType.DisarmerStateChanger;

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