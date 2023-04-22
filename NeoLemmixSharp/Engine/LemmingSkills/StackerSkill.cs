namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class StackerSkill : LemmingSkill
{
    public static StackerSkill Instance { get; } = new();

    private StackerSkill()
    {
    }

    public override int LemmingSkillId => 17;
    public override string LemmingSkillName => "stacker";

    public override bool CanAssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }
}