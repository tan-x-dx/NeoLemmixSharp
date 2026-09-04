using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class RotateClockwiseSkill : LemmingSkill
{
    public static readonly RotateClockwiseSkill Instance = new();

    private RotateClockwiseSkill()
        : base(
            LemmingSkillType.RotateClockwiseSkill,
            LemmingSkillConstants.RotateClockwiseSkillName)
    {
    }

    public override void AssignToLemming(Lemming lemming)
    {
        RotateClockwiseAction.TransitionLemmingToAction(lemming, false);
    }

    protected override LemmingActionTypeSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedRotationSkill;
}
