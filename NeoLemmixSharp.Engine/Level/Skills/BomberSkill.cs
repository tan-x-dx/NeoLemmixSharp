using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class BomberSkill
{
    public static bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.CountDownTimer == 0 && LemmingSkillType.BomberSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType);
    }

    public static void AssignToLemming(Lemming lemming)
    {
        var levelParameters = LevelScreen.LevelParameters;
        var countDownTimer = levelParameters.GetLemmingCountDownTimer(lemming);
        var displayTimer = levelParameters.Contains(LevelParameters.TimedBombers);

        lemming.SetCountDownAction(countDownTimer, LemmingActionType.ExploderAction, displayTimer);
    }

    public static IEnumerable<LemmingActionType> GetActionsThatCanBeAssigned() => LemmingSkill.GetActionsThatCanBeAssignedPermanentSkill();
}
