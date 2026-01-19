using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class NullifyFallDistanceLemmingBehaviour : LemmingBehaviour
{
    public NullifyFallDistanceLemmingBehaviour()
        : base(LemmingBehaviourType.NullifyLemmingFallDistance)
    {
    }

    protected override void PerformInternalBehaviour(Lemming lemming)
    {
        lemming.DistanceFallen = 0;
    }
}
