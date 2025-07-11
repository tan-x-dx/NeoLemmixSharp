using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.IO.Data.Level.Gadgets;

public sealed class GadgetLinkData
{
    public required GadgetIdentifier SourceGadgetIdentifier { get; init; }
    public required int SourceGadgetStateId { get; init; }
    public required GadgetIdentifier TargetGadgetIdentifier { get; init; }
    public required GadgetInputName TargetGadgetInputName { get; init; }

    internal GadgetLinkData()
    {
    }
}
