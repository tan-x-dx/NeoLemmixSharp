using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class SliderSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly SliderSkill Instance = new();

    private SliderSkill()
        : base(
            LevelConstants.SliderSkillId,
            LevelConstants.SliderSkillName)
    {
    }

    public int LemmingStateChangerId => LemmingStateChangerHelper.SliderStateChangerId;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.IsSlider && ActionIsAssignable(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsSlider = true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill();


    public void SetLemmingState(LemmingState lemmingState, bool status)
    {
        lemmingState.IsSlider = status;
    }

    public void ToggleLemmingState(LemmingState lemmingState)
    {
        lemmingState.IsSlider = !lemmingState.IsSlider;
    }

    public bool IsApplied(LemmingState lemmingState)
    {
        return lemmingState.IsSlider;
    }
}