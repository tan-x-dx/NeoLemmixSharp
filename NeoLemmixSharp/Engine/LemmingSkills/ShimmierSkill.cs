namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class ShimmierSkill : LemmingSkill
{
    public static ShimmierSkill Instance { get; } = new();

    private ShimmierSkill()
    {
    }

    public override int LemmingSkillId => 15;
    public override string LemmingSkillName => "shimmier";

    public override bool CanAssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }
}