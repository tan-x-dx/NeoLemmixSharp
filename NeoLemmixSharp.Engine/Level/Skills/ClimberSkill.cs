using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class ClimberSkill : LemmingSkill, IPermanentSkill
{
    public static ClimberSkill Instance { get; } = new();

    private ClimberSkill()
    {
    }

    public override int Id => Global.ClimberSkillId;
    public override string LemmingSkillName => "climber";

    public override bool IsClassicSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.IsClimber && ActionIsAssignable(lemming);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        lemming.State.IsClimber = true;
        return true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill();

    public void SetPermanentSkill(Lemming lemming, bool status)
    {
        lemming.State.IsClimber = status;
    }

    public void TogglePermanentSkill(Lemming lemming)
    {
        var isClimber = lemming.State.IsClimber;
        lemming.State.IsClimber = !isClimber;
    }
}