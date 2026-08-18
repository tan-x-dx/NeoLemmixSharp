using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

/// <summary>
/// Logically equivalent to null, but null references suck
/// </summary>
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
