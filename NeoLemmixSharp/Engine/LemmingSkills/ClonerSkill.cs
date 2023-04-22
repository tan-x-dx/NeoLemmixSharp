namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class ClonerSkill : LemmingSkill
{
    public static ClonerSkill Instance { get; } = new();

    private ClonerSkill()
    {
    }

    public override int LemmingSkillId => 5;
    public override string LemmingSkillName => "cloner";

    public override bool CanAssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }
}