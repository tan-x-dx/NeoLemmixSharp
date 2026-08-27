using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public static class DisarmerAction
{
    public static bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        lemming.DisarmingFrames--;
        if (lemming.DisarmingFrames <= 0)
        {
            if (lemming.NextActionType == LemmingActionType.NoneAction)
            {
                WalkerAction.TransitionLemmingToAction(lemming, false);
            }
            else
            {
                LemmingAction.TransitionLemmingToAction(lemming, false, lemming.NextActionType);
                lemming.SetNextActionType(LemmingActionType.NoneAction);
            }
        }
        else if ((lemming.PhysicsFrame & 7) == 0)
        {
            // ?? CueSoundEffect(SFX_FIXING, L.Position); ??
        }

        return false;
    }

    public static void TransitionLemmingToAction(Lemming lemming, bool turnAround) => LemmingActionType.DisarmerAction.DoMainTransitionActions(lemming, turnAround);
}
