namespace NeoLemmixSharp.Engine.Skills;

public sealed class FloaterSkill : LemmingSkill
{
    public FloaterSkill(int originalNumberOfSkillsAvailable) : base(originalNumberOfSkillsAvailable)
    {
    }

    public override int LemmingSkillId => 9;
    public override string LemmingSkillName => "floater";
    public override bool IsPermanentSkill => true;
    public override bool IsClassicSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !(lemming.IsGlider || lemming.IsFloater) && lemming.CurrentAction.CanBeAssignedPermanentSkill;
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        lemming.IsFloater = true;
        return true;
    }
}