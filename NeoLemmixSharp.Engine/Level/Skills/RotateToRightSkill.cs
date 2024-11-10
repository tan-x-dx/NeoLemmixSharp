using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class RotateToRightSkill : LemmingSkill
{
    public static readonly RotateToRightSkill Instance = new();

    private RotateToRightSkill()
        : base(
            EngineConstants.RotateToRightSkillId,
            EngineConstants.RotateToRightSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.Orientation != RightOrientation.Instance && ActionIsAssignable(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        if (orientation == DownOrientation.Instance)
        {
            RotateCounterclockwiseAction.Instance.TransitionLemmingToAction(lemming, false);
            return;
        }

        if (orientation == UpOrientation.Instance)
        {
            RotateClockwiseAction.Instance.TransitionLemmingToAction(lemming, false);
            return;
        }

        if (orientation == LeftOrientation.Instance)
        {
            RotateHalfAction.Instance.TransitionLemmingToAction(lemming, false);
            return;
        }
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        yield return WalkerAction.Instance;
        yield return ShruggerAction.Instance;
        yield return PlatformerAction.Instance;
        yield return BuilderAction.Instance;
        yield return StackerAction.Instance;
        yield return BasherAction.Instance;
        yield return FencerAction.Instance;
        yield return MinerAction.Instance;
        yield return DiggerAction.Instance;
        yield return LasererAction.Instance;
    }
}