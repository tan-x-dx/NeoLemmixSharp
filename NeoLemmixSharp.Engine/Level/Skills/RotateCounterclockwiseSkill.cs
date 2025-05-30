using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class RotateCounterclockwiseSkill : LemmingSkill
{
    public static readonly RotateCounterclockwiseSkill Instance = new();

    private RotateCounterclockwiseSkill()
        : base(
            LemmingSkillConstants.RotateCounterclockwiseSkillId,
            LemmingSkillConstants.RotateCounterclockwiseSkillName)
    {
    }

    public override void AssignToLemming(Lemming lemming)
    {
        RotateCounterclockwiseAction.Instance.TransitionLemmingToAction(lemming, false);
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedRotationSkill;
}