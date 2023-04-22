namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class PlatformerSkill : LemmingSkill
{
    public static PlatformerSkill Instance { get; } = new();

    private PlatformerSkill()
    {
    }

    public override int LemmingSkillId => 14;
    public override string LemmingSkillName => "platformer";

    public override bool CanAssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }
}