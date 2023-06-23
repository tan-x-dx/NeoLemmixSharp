namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class BomberSkill : LemmingSkill
{
    public BomberSkill(int originalNumberOfSkillsAvailable) : base(originalNumberOfSkillsAvailable)
    {
    }

    public override int LemmingSkillId => 2;
    public override string LemmingSkillName => "bomber";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.CurrentAction.CanBeAssignedPermanentSkill;
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        /*
            lemming.ExplosionTimer : = 1;
            lemming.TimerToStone : = False;
            lemming.HideCountdown : = True;
            */

        return true;
    }
}