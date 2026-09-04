using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class RotateHalfSkill : LemmingSkill
{
    public static readonly RotateHalfSkill Instance = new();

    private RotateHalfSkill()
        : base(
            LemmingSkillType.RotateHalfSkill,
            LemmingSkillConstants.RotateHalfSkillName)
    {
    }

    public override void AssignToLemming(Lemming lemming)
    {
        RotateHalfAction.TransitionLemmingToAction(lemming, false);
    }

    protected override LemmingActionTypeSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedRotationSkill;
}
