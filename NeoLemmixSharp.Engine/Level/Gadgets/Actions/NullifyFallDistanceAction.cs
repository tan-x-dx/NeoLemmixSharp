using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class NullifyFallDistanceAction : IGadgetAction
{
    public GadgetActionType ActionType => GadgetActionType.NullifyFallDistance;

    public void PerformAction(Lemming lemming)
    {
        lemming.DistanceFallen = 0;
    }
}
