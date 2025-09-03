using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

namespace NeoLemmixSharp.IO.Data.Level.Gadget.LogicGateGadget;

public sealed class LogicGateGadgetStateInstanceData : IGadgetStateInstanceData
{
    public static readonly LogicGateGadgetStateInstanceData Instance = new();

    public GadgetStateName OverrideStateName => default;
    public ReadOnlySpan<GadgetTriggerData> CustomTriggers => [];
    public ReadOnlySpan<GadgetBehaviourData> CustomBehaviours => [];
    public ReadOnlySpan<GadgetTriggerBehaviourLink> CustomTriggerBehaviourLinks => [];

    private LogicGateGadgetStateInstanceData()
    {
    }
}
