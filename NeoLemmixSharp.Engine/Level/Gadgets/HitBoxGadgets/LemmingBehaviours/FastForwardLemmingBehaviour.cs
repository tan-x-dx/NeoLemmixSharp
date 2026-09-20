using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class FastForwardLemmingBehaviour : LemmingBehaviour
{
    private readonly uint _fastForwardTime;

    public FastForwardLemmingBehaviour(uint fastForwardTime) : base(LemmingBehaviourType.SetLemmingFastForward)
    {
        _fastForwardTime = fastForwardTime;
    }

    protected override void PerformInternalBehaviour(Lemming lemming)
    {
        lemming.SetFastForwardTime(_fastForwardTime);
    }
}
