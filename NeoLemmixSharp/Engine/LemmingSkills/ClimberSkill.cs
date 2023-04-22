namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class ClimberSkill : LemmingSkill
{
    public static ClimberSkill Instance { get; } = new();

    private ClimberSkill()
    {
    }

    public override int LemmingSkillId => 4;
    public override string LemmingSkillName => "climber";

    public override bool CanAssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }
}