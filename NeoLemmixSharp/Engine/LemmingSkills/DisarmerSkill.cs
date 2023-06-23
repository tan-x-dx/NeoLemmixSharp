namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class DisarmerSkill : LemmingSkill
{
    public DisarmerSkill(int originalNumberOfSkillsAvailable) : base(originalNumberOfSkillsAvailable)
    {
    }

    public override int LemmingSkillId => 7;
    public override string LemmingSkillName => "disarmer";
    public override bool IsPermanentSkill => true;
    public override bool IsClassicSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.IsDisarmer && lemming.CurrentAction.CanBeAssignedPermanentSkill;
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        lemming.IsDisarmer = true;
        return true;
    }
}