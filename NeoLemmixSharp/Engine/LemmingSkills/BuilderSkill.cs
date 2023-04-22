namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class BuilderSkill : LemmingSkill
{
    public static BuilderSkill Instance { get; } = new();

    private BuilderSkill()
    {
    }

    public override int LemmingSkillId => 3;
    public override string LemmingSkillName => "builder";

    public override bool CanAssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }
}