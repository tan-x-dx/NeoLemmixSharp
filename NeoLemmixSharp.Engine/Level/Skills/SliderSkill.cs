using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class SliderSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly SliderSkill Instance = new();

    private SliderSkill()
    {
    }

    public override int Id => LevelConstants.SliderSkillId;
    public override string LemmingSkillName => "slider";
    public override bool IsClassicSkill => false;
    public int LemmingStateChangerId => LemmingStateChangerHelpers.SliderStateChangerId;

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
        var isSlider = lemmingState.IsSlider;
        lemmingState.IsSlider = !isSlider;
    }

    public bool IsApplied(LemmingState lemmingState)
    {
        return lemmingState.IsSlider;
    }
}