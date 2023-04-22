namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class SwimmerSkill : LemmingSkill
{
    public static SwimmerSkill Instance { get; } = new();

    private SwimmerSkill()
    {
    }

    public override int LemmingSkillId => 19;
    public override string LemmingSkillName => "swimmer";

    public override bool CanAssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }
}