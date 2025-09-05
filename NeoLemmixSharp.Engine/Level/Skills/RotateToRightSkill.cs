using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class RotateToRightSkill : LemmingSkill
{
    public static readonly RotateToRightSkill Instance = new();

    private RotateToRightSkill()
        : base(
            LemmingSkillConstants.RotateToRightSkillId,
            LemmingSkillConstants.RotateToRightSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.Data.Orientation != Orientation.Right && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        var orientation = lemming.Data.Orientation;
        if (orientation == Orientation.Down)
        {
            RotateCounterclockwiseAction.Instance.TransitionLemmingToAction(lemming, false);
            return;
        }

        if (orientation == Orientation.Up)
        {
            RotateClockwiseAction.Instance.TransitionLemmingToAction(lemming, false);
            return;
        }

        if (orientation == Orientation.Left)
        {
            RotateHalfAction.Instance.TransitionLemmingToAction(lemming, false);
            return;
        }
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedRotationSkill;
}