using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class GliderSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly GliderSkill Instance = new();

    private GliderSkill()
    {
    }

    public override int Id => LevelConstants.GliderSkillId;
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

    public void SetLemmingState(LemmingState lemmingState, bool status)
    {
        lemmingState.IsGlider = status;
    }

    public void ToggleLemmingState(LemmingState lemmingState)
    {
        var isGlider = lemmingState.IsGlider;
        lemmingState.IsGlider = !isGlider;
    }

    public bool IsApplied(LemmingState lemmingState)
    {
        return lemmingState.IsGlider;
    }
}