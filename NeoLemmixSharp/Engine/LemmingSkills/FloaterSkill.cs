namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class FloaterSkill : LemmingSkill
{
    public static FloaterSkill Instance { get; } = new();

    private FloaterSkill()
    {
    }

    public override int LemmingSkillId => 9;
    public override string LemmingSkillName => "floater";
    public override bool IsPermanentSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !(lemming.IsGlider || lemming.IsFloater) && LemmingActionCanBeAssignedPermanentSkill(lemming);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        lemming.IsFloater = true;
        return true;
    }
}