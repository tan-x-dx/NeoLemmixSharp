namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class MinerSkill : LemmingSkill
{
    public static MinerSkill Instance { get; } = new();

    private MinerSkill()
    {
    }

    public override int LemmingSkillId => 13;
    public override string LemmingSkillName => "miner";

    public override bool CanAssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }
}