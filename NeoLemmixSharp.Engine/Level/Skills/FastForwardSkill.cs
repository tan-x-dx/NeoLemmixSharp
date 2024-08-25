using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class FastForwardSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly FastForwardSkill Instance = new();

    private FastForwardSkill()
        : base(
            LevelConstants.FastForwardSkillId,
            LevelConstants.FastForwardSkillName)
    {
    }

    public int LemmingStateChangerId => LemmingStateChangerHelpers.FastForwardStateChangerId;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.IsPermanentFastForwards && ActionIsAssignable(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsPermanentFastForwards = true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        yield return WalkerAction.Instance;
        yield return ClimberAction.Instance;
        yield return FloaterAction.Instance;
        yield return BlockerAction.Instance;
        yield return BuilderAction.Instance;
        yield return BasherAction.Instance;
        yield return MinerAction.Instance;
        yield return DiggerAction.Instance;
        yield return PlatformerAction.Instance;
        yield return StackerAction.Instance;
        yield return FencerAction.Instance;
        yield return GliderAction.Instance;
        yield return JumperAction.Instance;
        yield return SwimmerAction.Instance;
        yield return ShimmierAction.Instance;
        yield return LasererAction.Instance;
        yield return SliderAction.Instance;
        yield return FallerAction.Instance;
        yield return AscenderAction.Instance;
        yield return ShruggerAction.Instance;
        yield return DrownerAction.Instance;
        yield return HoisterAction.Instance;
        yield return DehoisterAction.Instance;
        yield return ReacherAction.Instance;
        yield return DisarmerAction.Instance;
        yield return ExiterAction.Instance;
        yield return ExploderAction.Instance;
        yield return OhNoerAction.Instance;
        yield return SplatterAction.Instance;
        yield return StonerAction.Instance;
        yield return VaporiserAction.Instance;
        yield return RotateClockwiseAction.Instance;
        yield return RotateCounterclockwiseAction.Instance;
        yield return RotateHalfAction.Instance;
    }

    public void SetLemmingState(LemmingState lemmingState, bool status)
    {
        lemmingState.IsPermanentFastForwards = status;
    }

    public void ToggleLemmingState(LemmingState lemmingState)
    {
        lemmingState.IsPermanentFastForwards = !lemmingState.IsPermanentFastForwards;
    }

    public bool IsApplied(LemmingState lemmingState)
    {
        return lemmingState.IsPermanentFastForwards;
    }
}