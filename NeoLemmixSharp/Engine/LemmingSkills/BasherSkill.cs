namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class BasherSkill : LemmingSkill
{
    public static BasherSkill Instance { get; } = new();

    private BasherSkill()
    {
    }

    public override int LemmingSkillId => 0;
    public override string LemmingSkillName => "basher";

    public override bool CanAssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }
}