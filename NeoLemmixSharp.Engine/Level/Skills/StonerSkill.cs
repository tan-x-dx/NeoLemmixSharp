using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class StonerSkill
{
    public static bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.CountDownTimer == 0 && LemmingSkillType.StonerSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType);
    }

    public static void AssignToLemming(Lemming lemming)
    {
        var levelParameters = LevelScreen.LevelParameters;
        var countDownTimer = levelParameters.GetLemmingCountDownTimer(lemming);

        lemming.SetCountDownAction(countDownTimer, LemmingActionType.StonerAction);
    }

    public static IEnumerable<LemmingActionType> GetActionsThatCanBeAssigned() => LemmingSkill.GetActionsThatCanBeAssignedPermanentSkill();
}
