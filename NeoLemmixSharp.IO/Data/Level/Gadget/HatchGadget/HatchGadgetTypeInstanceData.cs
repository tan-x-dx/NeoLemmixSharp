using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.IO.Data.Level.Gadget.HatchGadget;

public sealed class HatchGadgetTypeInstanceData : IGadgetTypeInstanceData
{
    public GadgetType GadgetType => GadgetType.HatchGadget;
    public required int InitialStateId { get; init; }
    public required HatchGadgetStateInstanceData[] GadgetStates { get; init; }

    public required int HatchGroupId { get; init; }
    public required int TribeId { get; init; }
    public required uint RawStateData { get; init; }
    public required int NumberOfLemmingsToRelease { get; init; }
}
