using NeoLemmixSharp.Engine.Engine.Actions;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class BomberSkill : LemmingSkill
{
    public static BomberSkill Instance { get; } = new();

    private BomberSkill()
    {
    }

    public override int Id => 4;
    public override string LemmingSkillName => "bomber";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => true;

    public override bool AssignToLemming(Lemming lemming)
    {
        /*
            lemming.ExplosionTimer : = 1;
            lemming.TimerToStone : = False;
            lemming.HideCountdown : = True;
            */

        return true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill();
}