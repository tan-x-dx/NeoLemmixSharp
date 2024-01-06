using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class ClimberSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly ClimberSkill Instance = new();

    private ClimberSkill()
    {
    }

    public override int Id => LevelConstants.ClimberSkillId;
    public override string LemmingSkillName => "climber";

    public override bool IsClassicSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.IsClimber && ActionIsAssignable(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsClimber = true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill();

    public void SetLemmingState(LemmingState lemmingState, bool status)
    {
        lemmingState.IsClimber = status;
    }

    public void ToggleLemmingState(LemmingState lemmingState)
    {
        var isClimber = lemmingState.IsClimber;
        lemmingState.IsClimber = !isClimber;
    }

    public bool IsApplied(LemmingState lemmingState)
    {
        return lemmingState.IsClimber;
    }
}