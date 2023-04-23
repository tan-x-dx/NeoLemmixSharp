namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class ClimberSkill : LemmingSkill
{
    public static ClimberSkill Instance { get; } = new();

    private ClimberSkill()
    {
    }

    public override int LemmingSkillId => 4;
    public override string LemmingSkillName => "climber";
    public override bool IsPermanentSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.IsClimber && LemmingActionCanBeAssignedPermanentSkill(lemming);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        lemming.IsClimber = true;
        return true;
    }
}