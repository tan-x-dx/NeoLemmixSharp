namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class BlockerSkill : LemmingSkill
{
    public static BlockerSkill Instance { get; } = new();

    private BlockerSkill()
    {
    }

    public override int LemmingSkillId => 1;
    public override string LemmingSkillName => "blocker";
    public override bool CanAssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }
}