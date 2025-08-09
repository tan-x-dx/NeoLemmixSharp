using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public readonly struct CauseAndEffectData
{
    public readonly int GadgetBehaviourId;
    public readonly int LemmingId;

    public CauseAndEffectData(int gadgetBehaviourId, int lemmingId)
    {
        GadgetBehaviourId = gadgetBehaviourId;
        LemmingId = lemmingId;
    }

    public CauseAndEffectData(int gadgetBehaviourId)
    {
        GadgetBehaviourId = gadgetBehaviourId;
        LemmingId = EngineConstants.NoLemmingCauseAndEffectId;
    }
}
