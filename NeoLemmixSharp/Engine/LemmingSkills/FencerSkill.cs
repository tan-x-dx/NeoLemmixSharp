namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class FencerSkill : LemmingSkill
{
    public static FencerSkill Instance { get; } = new();

    private FencerSkill()
    {
    }

    public override int LemmingSkillId => 8;
    public override string LemmingSkillName => "fencer";

    public override bool CanAssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }
}