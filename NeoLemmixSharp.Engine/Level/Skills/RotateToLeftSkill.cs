using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class RotateToLeftSkill : LemmingSkill
{
    public static readonly RotateToLeftSkill Instance = new();

    private RotateToLeftSkill()
        : base(
            EngineConstants.RotateToLeftSkillId,
            EngineConstants.RotateToLeftSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.Orientation != Orientation.Left && ActionIsAssignable(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        if (orientation == Orientation.Down)
        {
            RotateClockwiseAction.Instance.TransitionLemmingToAction(lemming, false);
            return;
        }

        if (orientation == Orientation.Right)
        {
            RotateHalfAction.Instance.TransitionLemmingToAction(lemming, false);
            return;
        }

        if (orientation == Orientation.Up)
        {
            RotateCounterclockwiseAction.Instance.TransitionLemmingToAction(lemming, false);
            return;
        }
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedRotationSkill;
}