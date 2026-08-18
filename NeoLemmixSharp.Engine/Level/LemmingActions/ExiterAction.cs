using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public static class ExiterAction
{
    public static bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        if (LevelScreen.LevelTimer.OutOfTime)
        {
            lemming.AnimationFrame--;
            lemming.PhysicsFrame--;

            //if UserSetNuking and (L.LemExplosionTimer <= 0) and (Index_LemmingToBeNuked > L.LemIndex) then
            //  Transition(L, baOhnoing);

            return false;
        }

        if (lemming.EndOfAnimation)
        {
            LevelScreen.LemmingManager.RemoveLemming(lemming, LemmingRemovalReason.Exit);
        }

        return false;
    }

    public static void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        LemmingActionType.ExiterAction.DoMainTransitionActions(lemming, turnAround);

        lemming.CountDownTimer = 0;
    }
}
