namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class NoneSkill : LemmingSkill
{
    /// <summary>
    /// Logically equivalent to null, but null references suck.
    /// </summary>
    public static NoneSkill Instance { get; } = new();

    private NoneSkill() : base(0)
    {
    }

    public override int LemmingSkillId => -1;
    public override string LemmingSkillName => "none";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return false;
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        return false;
    }
}