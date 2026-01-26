using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class FastForwardLemmingBehaviour : LemmingBehaviour
{
    private readonly int _fastForwardTime;

    public FastForwardLemmingBehaviour(int fastForwardTime) : base(LemmingBehaviourType.SetLemmingFastForward)
    {
        _fastForwardTime = fastForwardTime;
    }

    protected override void PerformInternalBehaviour(Lemming lemming)
    {
        lemming.SetFastForwardTime(_fastForwardTime);
    }
}
