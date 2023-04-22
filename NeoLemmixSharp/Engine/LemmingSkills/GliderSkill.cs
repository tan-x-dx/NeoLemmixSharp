namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class GliderSkill : LemmingSkill
{
    public static GliderSkill Instance { get; } = new();

    private GliderSkill()
    {
    }

    public override int LemmingSkillId => 10;
    public override string LemmingSkillName => "glider";

    public override bool CanAssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }
}