using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class SetTribeLemmingBehaviour : LemmingBehaviour
{
    private readonly int _tribeId;

    public SetTribeLemmingBehaviour(int tribeId)
        : base(LemmingBehaviourType.SetLemmingTribe)
    {
        _tribeId = tribeId;
    }

    protected override void PerformInternalBehaviour(Lemming lemming)
    {
        lemming.State.SetTribeAffiliation(_tribeId);
    }
}
