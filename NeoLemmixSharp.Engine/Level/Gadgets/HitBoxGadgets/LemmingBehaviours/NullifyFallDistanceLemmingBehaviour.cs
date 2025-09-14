using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class NullifyFallDistanceLemmingBehaviour : LemmingBehaviour
{
    public NullifyFallDistanceLemmingBehaviour()
        : base(LemmingBehaviourType.NullifyLemmingFallDistance)
    {
    }

    protected override void PerformInternalBehaviour(int triggerData)
    {
        var lemming = GetLemming(triggerData);
        lemming.DistanceFallen = 0;
    }
}
