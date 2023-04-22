namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class WalkerSkill : LemmingSkill
{
    public static WalkerSkill Instance { get; } = new();

    private WalkerSkill()
    {
    }

    public override int LemmingSkillId => 20;
    public override string LemmingSkillName => "walker";

    public override bool CanAssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }
}