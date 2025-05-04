using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using static NeoLemmixSharp.Engine.Level.Skills.ILemmingStateChanger;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class SliderSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly SliderSkill Instance = new();

    private SliderSkill()
        : base(
            EngineConstants.SliderSkillId,
            EngineConstants.SliderSkillName)
    {
    }

    public StateChangerType LemmingStateChangerType => StateChangerType.SliderStateChanger;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.IsSlider && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsSlider = true;
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;

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