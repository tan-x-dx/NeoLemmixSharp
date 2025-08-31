using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

namespace NeoLemmixSharp.IO.Data.Level.Gadget;

public sealed class GadgetLinkData
{
    public required GadgetIdentifier SourceGadgetIdentifier { get; init; }
    public required int SourceGadgetStateId { get; init; }
    public required GadgetBehaviourName SourceLinkBehaviourName { get; init; }
    public required GadgetIdentifier TargetGadgetIdentifier { get; init; }
    public required GadgetTriggerName TargetGadgetInputName { get; init; }

    internal GadgetLinkData()
    {
    }
}
