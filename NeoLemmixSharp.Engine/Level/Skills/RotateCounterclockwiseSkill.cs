using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class RotateCounterclockwiseSkill
{
    public static bool CanAssignToLemming(Lemming lemming)
    {
        return LemmingSkillType.RotateClockwiseSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType);
    }

    public static void AssignToLemming(Lemming lemming)
    {
        RotateCounterclockwiseAction.TransitionLemmingToAction(lemming, false);
    }

    public static IEnumerable<LemmingActionType> GetActionsThatCanBeAssigned() => LemmingSkill.GetActionsThatCanBeAssignedRotationSkill();
}
