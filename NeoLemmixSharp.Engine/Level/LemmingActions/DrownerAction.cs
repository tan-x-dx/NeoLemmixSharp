using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public static class DrownerAction
{
    public static bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        /*   var gadgetManager = LevelScreen.GadgetManager;
           Span<uint> scratchSpaceSpan = stackalloc uint[gadgetManager.ScratchSpaceSize];
           if (!gadgetManager.HasGadgetWithBehaviourAtLemmingPosition(scratchSpaceSpan, lemming, WaterGadgetBehaviour.Instance))
           {
               WalkerAction.Instance.TransitionLemmingToAction(lemming, false);

               return true;
           }*/

        if (lemming.EndOfAnimation)
        {
            LevelScreen.LemmingManager.RemoveLemming(lemming, LemmingRemovalReason.DeathDrown);
        }

        return false;
    }

    public static void TransitionLemmingToAction(Lemming lemming, bool turnAround) => LemmingActionType.DrownerAction.DoMainTransitionActions(lemming, turnAround);
}
