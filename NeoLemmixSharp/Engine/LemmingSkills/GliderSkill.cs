namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class GliderSkill : LemmingSkill
{
    public GliderSkill(int originalNumberOfSkillsAvailable) : base(originalNumberOfSkillsAvailable)
    {
    }

    public override int LemmingSkillId => 10;
    public override string LemmingSkillName => "glider";
    public override bool IsPermanentSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !(lemming.IsGlider || lemming.IsFloater) && lemming.CurrentAction.CanBeAssignedPermanentSkill;
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        lemming.IsGlider = true;
        return true;
    }
}