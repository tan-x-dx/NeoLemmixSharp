using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public static class ShruggerAction
{
    public static bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        if (lemming.EndOfAnimation)
        {
            WalkerAction.TransitionLemmingToAction(lemming, false);
        }

        return true;
    }

    public static void TransitionLemmingToAction(Lemming lemming, bool turnAround) => LemmingActionType.ShruggerAction.DoMainTransitionActions(lemming, turnAround);
}
