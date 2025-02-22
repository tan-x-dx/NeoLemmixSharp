using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using static NeoLemmixSharp.Engine.Level.Skills.ILemmingStateChanger;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class SwimmerSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly SwimmerSkill Instance = new();

    private SwimmerSkill()
        : base(
            EngineConstants.SwimmerSkillId,
            EngineConstants.SwimmerSkillName)
    {
    }

    public StateChangerType LemmingStateChangerType => StateChangerType.SwimmerStateChanger;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.HasLiquidAffinity && ActionIsAssignable(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsSwimmer = true;
        if (lemming.CurrentAction == DrownerAction.Instance)
        {
            SwimmerAction.Instance.TransitionLemmingToAction(lemming, false);
        }
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned()
    {
        var result = LemmingAction.CreateBitArraySet();

        result.Add(AscenderAction.Instance);
        result.Add(BasherAction.Instance);
        result.Add(BlockerAction.Instance);
        result.Add(BuilderAction.Instance);
        result.Add(ClimberAction.Instance);
        result.Add(DehoisterAction.Instance);
        result.Add(DiggerAction.Instance);
        result.Add(DisarmerAction.Instance);
        result.Add(DrownerAction.Instance);
        result.Add(FallerAction.Instance);
        result.Add(FencerAction.Instance);
        result.Add(FloaterAction.Instance);
        result.Add(GliderAction.Instance);
        result.Add(HoisterAction.Instance);
        result.Add(JumperAction.Instance);
        result.Add(LasererAction.Instance);
        result.Add(MinerAction.Instance);
        result.Add(PlatformerAction.Instance);
        result.Add(ReacherAction.Instance);
        result.Add(ShimmierAction.Instance);
        result.Add(ShruggerAction.Instance);
        result.Add(SliderAction.Instance);
        result.Add(StackerAction.Instance);
        result.Add(SwimmerAction.Instance);
        result.Add(WalkerAction.Instance);
        result.Add(RotateClockwiseAction.Instance);
        result.Add(RotateCounterclockwiseAction.Instance);
        result.Add(RotateHalfAction.Instance);

        return result;
    }

    public void SetLemmingState(LemmingState lemmingState, bool status)
    {
        lemmingState.IsSwimmer = status;
    }

    public void ToggleLemmingState(LemmingState lemmingState)
    {
        lemmingState.IsSwimmer = !lemmingState.IsSwimmer;
    }

    public bool IsApplied(LemmingState lemmingState)
    {
        return lemmingState.IsSwimmer;
    }
}