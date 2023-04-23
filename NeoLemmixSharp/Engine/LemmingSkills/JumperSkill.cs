namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class JumperSkill : LemmingSkill
{
    public static JumperSkill Instance { get; } = new();

    private JumperSkill()
    {
    }

    public override int LemmingSkillId => 11;
    public override string LemmingSkillName => "jumper";
    public override bool IsPermanentSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }
}