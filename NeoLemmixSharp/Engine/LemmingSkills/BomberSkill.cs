namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class BomberSkill : LemmingSkill
{
    public static BomberSkill Instance { get; } = new();

    private BomberSkill()
    {
    }

    public override int LemmingSkillId => 2;
    public override string LemmingSkillName => "bomber";

    public override bool CanAssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }
}