using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public static class VaporiserAction
{
    public static bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        if (lemming.EndOfAnimation)
        {
            LevelScreen.LemmingManager.RemoveLemming(lemming, LemmingRemovalReason.DeathFire);
        }

        return false;
    }

    public static void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        LemmingActionType.VaporiserAction.DoMainTransitionActions(lemming, turnAround);

        lemming.CountDownTimer = 0;
    }
}
