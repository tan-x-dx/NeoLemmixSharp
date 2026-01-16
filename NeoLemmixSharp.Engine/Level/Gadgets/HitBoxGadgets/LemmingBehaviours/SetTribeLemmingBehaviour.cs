using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class SetTribeLemmingBehaviour : LemmingBehaviour
{
    private readonly int _tribeId;

    public SetTribeLemmingBehaviour(int tribeId)
        : base(LemmingBehaviourType.SetLemmingTribe)
    {
        _tribeId = tribeId;
    }

    protected override void PerformInternalBehaviour(int triggerData)
    {
        var lemming = GetLemming(triggerData);
        lemming.State.SetTribeAffiliation(_tribeId);
    }
}
