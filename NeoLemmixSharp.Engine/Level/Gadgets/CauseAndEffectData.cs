namespace NeoLemmixSharp.Engine.Level.Gadgets;

public readonly struct CauseAndEffectData(int gadgetBehaviourId, int triggerData)
{
    public readonly int GadgetBehaviourId = gadgetBehaviourId;
    public readonly int TriggerData = triggerData;
}
