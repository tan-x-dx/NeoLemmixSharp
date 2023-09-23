using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class GliderSkill : LemmingSkill, IPermanentSkill
{
    public static GliderSkill Instance { get; } = new();

    private GliderSkill()
    {
    }

    public override int Id => Global.GliderSkillId;
    public override string LemmingSkillName => "glider";
    public override bool IsClassicSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !(lemming.State.IsGlider || lemming.State.IsFloater) && ActionIsAssignable(lemming);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        lemming.State.IsGlider = true;
        return true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill();

    public void SetPermanentSkill(Lemming lemming, bool status)
    {
        lemming.State.IsGlider = status;
    }

    public void TogglePermanentSkill(Lemming lemming)
    {
        var isGlider = lemming.State.IsGlider;
        lemming.State.IsGlider = !isGlider;
    }
}