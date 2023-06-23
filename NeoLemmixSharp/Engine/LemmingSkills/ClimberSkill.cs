namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class ClimberSkill : LemmingSkill
{
    public ClimberSkill(int originalNumberOfSkillsAvailable) : base(originalNumberOfSkillsAvailable)
    {
    }

    public override int LemmingSkillId => 4;
    public override string LemmingSkillName => "climber";
    public override bool IsPermanentSkill => true;
    public override bool IsClassicSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.IsClimber && lemming.CurrentAction.CanBeAssignedPermanentSkill;
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        lemming.IsClimber = true;
        return true;
    }
}