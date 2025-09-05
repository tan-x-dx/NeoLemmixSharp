using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class RotateToDownSkill : LemmingSkill
{
    public static readonly RotateToDownSkill Instance = new();

    private RotateToDownSkill()
        : base(
            LemmingSkillConstants.RotateToDownSkillId,
            LemmingSkillConstants.RotateToDownSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.Data.Orientation != Orientation.Down && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        var orientation = lemming.Data.Orientation;
        if (orientation == Orientation.Right)
        {
            RotateClockwiseAction.Instance.TransitionLemmingToAction(lemming, false);
            return;
        }

        if (orientation == Orientation.Up)
        {
            RotateHalfAction.Instance.TransitionLemmingToAction(lemming, false);
            return;
        }

        if (orientation == Orientation.Left)
        {
            RotateCounterclockwiseAction.Instance.TransitionLemmingToAction(lemming, false);
            return;
        }
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedRotationSkill;
}