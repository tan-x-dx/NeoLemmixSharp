namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class LasererSkill : LemmingSkill
{
    public static LasererSkill Instance { get; } = new();

    private LasererSkill()
    {
    }

    public override int LemmingSkillId => 12;
    public override string LemmingSkillName => "laserer";

    public override bool CanAssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }
}