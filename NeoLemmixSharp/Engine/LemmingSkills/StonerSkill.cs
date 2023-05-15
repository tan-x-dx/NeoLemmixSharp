namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class StonerSkill : LemmingSkill
{
    public StonerSkill(int originalNumberOfSkillsAvailable) : base(originalNumberOfSkillsAvailable)
    {
    }

    public override int LemmingSkillId => 18;
    public override string LemmingSkillName => "stoner";
    public override bool IsPermanentSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.CurrentAction.CanBeAssignedPermanentSkill;
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        /*
            lemming.LemExplosionTimer : = 1;
            lemming.LemTimerToStone : = True;
            lemming.LemHideCountdown : = True;
            */

        return true;
    }
}