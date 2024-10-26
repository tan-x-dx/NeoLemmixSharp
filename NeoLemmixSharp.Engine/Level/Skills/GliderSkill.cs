using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class GliderSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly GliderSkill Instance = new();

    private GliderSkill()
        : base(
            LevelConstants.GliderSkillId,
            LevelConstants.GliderSkillName)
    {
    }

    public int LemmingStateChangerId => LemmingStateChangerHelper.GliderStateChangerId;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.HasSpecialFallingBehaviour && ActionIsAssignable(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsGlider = true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill();

    public void SetLemmingState(LemmingState lemmingState, bool status)
    {
        lemmingState.IsGlider = status;
    }

    public void ToggleLemmingState(LemmingState lemmingState)
    {
        lemmingState.IsGlider = !lemmingState.IsGlider;
    }

    public bool IsApplied(LemmingState lemmingState)
    {
        return lemmingState.IsGlider;
    }
}