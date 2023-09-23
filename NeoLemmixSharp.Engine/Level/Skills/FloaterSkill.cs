using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class FloaterSkill : LemmingSkill, IPermanentSkill
{
    public static FloaterSkill Instance { get; } = new();

    private FloaterSkill()
    {
    }

    public override int Id => Global.FloaterSkillId;
    public override string LemmingSkillName => "floater";
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

    public void SetPermanentSkill(Lemming lemming, bool status)
    {
        lemming.State.IsFloater = status;
    }

    public void TogglePermanentSkill(Lemming lemming)
    {
        var isFloater = lemming.State.IsFloater;
        lemming.State.IsFloater = !isFloater;
    }
}