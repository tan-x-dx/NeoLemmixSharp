using NeoLemmixSharp.Engine.Engine.Actions;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class FloaterSkill : LemmingSkill
{
    public static FloaterSkill Instance { get; } = new();

    private FloaterSkill()
    {
    }

    public override int Id => 2;
    public override string LemmingSkillName => "floater";
    public override bool IsPermanentSkill => true;
    public override bool IsClassicSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !(lemming.IsGlider || lemming.IsFloater) && ActionIsAssignable(lemming);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        lemming.IsFloater = true;
        return true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill();
}