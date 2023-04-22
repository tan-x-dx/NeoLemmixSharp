namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class SliderSkill : LemmingSkill
{
    public static SliderSkill Instance { get; } = new();

    private SliderSkill()
    {
    }

    public override int LemmingSkillId => 16;
    public override string LemmingSkillName => "slider";

    public override bool CanAssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }
}