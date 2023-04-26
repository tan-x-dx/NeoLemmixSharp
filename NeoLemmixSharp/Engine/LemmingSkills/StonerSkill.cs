namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class StonerSkill : LemmingSkill
{
    public static StonerSkill Instance { get; } = new();

    private StonerSkill()
    {
    }

    public override int LemmingSkillId => 18;
    public override string LemmingSkillName => "stoner";
    public override bool IsPermanentSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return LemmingActionCanBeAssignedPermanentSkill(lemming);
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