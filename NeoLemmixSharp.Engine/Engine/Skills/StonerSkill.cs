using NeoLemmixSharp.Engine.Engine.LemmingActions;
using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class StonerSkill : LemmingSkill
{
    public static StonerSkill Instance { get; } = new();

    private StonerSkill()
    {
    }

    public override int Id => GameConstants.StonerSkillId;
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

        throw new NotImplementedException();
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill();
}