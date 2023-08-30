using NeoLemmixSharp.Engine.Engine.LemmingActions;
using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class FloaterSkill : LemmingSkill
{
    public static FloaterSkill Instance { get; } = new();

    private FloaterSkill()
    {
    }

    public override int Id => GameConstants.FloaterSkillId;
    public override string LemmingSkillName => "floater";
    public override bool IsPermanentSkill => true;
    public override bool IsClassicSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !(lemming.State.IsGlider || lemming.State.IsFloater) && ActionIsAssignable(lemming);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        lemming.State.IsFloater = true;
        return true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill();
}