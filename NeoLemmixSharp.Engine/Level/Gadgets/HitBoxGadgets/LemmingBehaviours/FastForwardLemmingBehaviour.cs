using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class FastForwardLemmingBehaviour : LemmingBehaviour
{
    private readonly int _fastForwardTime;

    public FastForwardLemmingBehaviour(int fastForwardTime) : base(LemmingBehaviourType.SetLemmingFastForward)
    {
        _fastForwardTime = fastForwardTime;
    }

    protected override void PerformInternalBehaviour(int lemmingId)
    {
        var lemming = GetLemming(lemmingId);
        lemming.FastForwardTime = _fastForwardTime;
    }
}
