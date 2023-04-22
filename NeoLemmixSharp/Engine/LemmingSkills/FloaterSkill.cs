namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class FloaterSkill : LemmingSkill
{
    public static FloaterSkill Instance { get; } = new();

    private FloaterSkill()
    {
    }

    public override int LemmingSkillId => 9;
    public override string LemmingSkillName => "floater";

    public override bool CanAssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }
}