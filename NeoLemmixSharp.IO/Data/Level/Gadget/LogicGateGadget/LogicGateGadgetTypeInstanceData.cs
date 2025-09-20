using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.IO.Data.Level.Gadget.LogicGateGadget;

public sealed class LogicGateGadgetTypeInstanceData : IGadgetTypeInstanceData
{
    public required GadgetType GadgetType { get; init; }
    public required int NumberOfInputs { get; init; }
}
