using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public static class NoneAction
{
    public static bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        return false;
    }

    public static void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
    }
}
