using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.LemmingBehaviours;

public sealed class NullifyFallDistanceBehaviour : LemmingBehaviour
{
    public NullifyFallDistanceBehaviour()
        : base(LemmingBehaviourType.NullifyFallDistance)
    {
    }

    public override void PerformBehaviour(Lemming lemming)
    {
        lemming.DistanceFallen = 0;
    }
}
