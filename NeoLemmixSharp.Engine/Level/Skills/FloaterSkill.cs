using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class FloaterSkill : LemmingSkill, ILemmingStateChanger
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

    public void SetLemmingState(LemmingState lemmingState, bool status)
    {
        lemmingState.IsFloater = status;
    }

    public void ToggleLemmingState(LemmingState lemmingState)
    {
        var isFloater = lemmingState.IsFloater;
        lemmingState.IsFloater = !isFloater;
    }

    public bool IsApplied(LemmingState lemmingState)
    {
        return lemmingState.IsFloater;
    }
}