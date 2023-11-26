using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class DisarmerSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly DisarmerSkill Instance = new();

    private DisarmerSkill()
    {
    }

    public override int Id => LevelConstants.DisarmerSkillId;
    public override string LemmingSkillName => "disarmer";
    public override bool IsClassicSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.IsDisarmer && ActionIsAssignable(lemming);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        lemming.State.IsDisarmer = true;
        return true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill();

    public void SetLemmingState(LemmingState lemmingState, bool status)
    {
        lemmingState.IsDisarmer = status;
    }

    public void ToggleLemmingState(LemmingState lemmingState)
    {
        var isDisarmer = lemmingState.IsDisarmer;
        lemmingState.IsDisarmer = !isDisarmer;
    }

    public bool IsApplied(LemmingState lemmingState)
    {
        return lemmingState.IsDisarmer;
    }
}