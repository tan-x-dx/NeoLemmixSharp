namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class DisarmerSkill : LemmingSkill
{
    public static DisarmerSkill Instance { get; } = new();

    private DisarmerSkill()
    {
    }

    public override int LemmingSkillId => 7;
    public override string LemmingSkillName => "disarmer";
    public override bool IsPermanentSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.IsDisarmer && LemmingActionCanBeAssignedPermanentSkill(lemming);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        throw new System.NotImplementedException();
    }
}