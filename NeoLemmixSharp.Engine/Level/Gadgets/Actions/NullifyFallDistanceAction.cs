using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class NullifyFallDistanceAction : GadgetAction
{
    public NullifyFallDistanceAction()
        : base(GadgetActionType.NullifyFallDistance)
    {
    }

    public override void PerformAction(Lemming lemming)
    {
        lemming.DistanceFallen = 0;
    }
}
