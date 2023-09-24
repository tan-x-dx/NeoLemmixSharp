using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class SliderSkill : LemmingSkill, ILemmingStateChanger
{
    public static SliderSkill Instance { get; } = new();

    private SliderSkill()
    {
    }

    public override int Id => Global.SliderSkillId;
    public override string LemmingSkillName => "slider";
    public override bool IsClassicSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.IsSlider && ActionIsAssignable(lemming);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        lemming.State.IsSlider = true;
        return true;
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
}