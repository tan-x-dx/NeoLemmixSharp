namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class DiggerSkill : LemmingSkill
{
    public static DiggerSkill Instance { get; } = new();

    private DiggerSkill()
    {
    }

    public override int LemmingSkillId => 6;
    public override string LemmingSkillName => "digger";

    public override bool CanAssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }
}