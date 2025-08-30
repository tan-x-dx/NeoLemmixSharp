using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class NullifyFallDistanceLemmingBehaviour : LemmingBehaviour
{
    public NullifyFallDistanceLemmingBehaviour()
        : base(LemmingBehaviourType.NullifyLemmingFallDistance)
    {
    }

    protected override void PerformInternalBehaviour(int lemmingId)
    {
        var lemming = GetLemming(lemmingId);
        lemming.DistanceFallen = 0;
    }
}
