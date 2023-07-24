using NeoLemmixSharp.Engine.Engine.Actions;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class StonerSkill : LemmingSkill
{
    public static StonerSkill Instance { get; } = new();

    private StonerSkill()
    {
    }

    public override int Id => 19;
    public override string LemmingSkillName => "stoner";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => false;

    public override bool AssignToLemming(Lemming lemming)
    {
        /*
            lemming.LemExplosionTimer : = 1;
            lemming.LemTimerToStone : = True;
            lemming.LemHideCountdown : = True;
            */

        return true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill();
}