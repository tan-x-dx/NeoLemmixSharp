using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class RotateToUpSkill : LemmingSkill
{
    public static readonly RotateToUpSkill Instance = new();

    private RotateToUpSkill()
        : base(
            LemmingSkillConstants.RotateToUpSkillId,
            LemmingSkillConstants.RotateToUpSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.Data.Orientation != Orientation.Up && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        var orientation = lemming.Data.Orientation;
        if (orientation == Orientation.Down)
        {
            RotateHalfAction.Instance.TransitionLemmingToAction(lemming, false);
            return;
        }

        if (orientation == Orientation.Right)
        {
            RotateCounterclockwiseAction.Instance.TransitionLemmingToAction(lemming, false);
            return;
        }

        if (orientation == Orientation.Left)
        {
            RotateClockwiseAction.Instance.TransitionLemmingToAction(lemming, false);
            return;
        }
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedRotationSkill;
}