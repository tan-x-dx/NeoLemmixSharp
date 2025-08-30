using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.IO.Data.Level.Gadgets.Hatch;

public sealed class HatchGadgetInstanceData : IGadgetInstanceData
{
    public GadgetType GadgetType => GadgetType.HatchGadget;
    public required GadgetIdentifier Identifier { get; init; }
    public required GadgetName OverrideName { get; init; }

    public required StyleIdentifier StyleIdentifier { get; init; }
    public required PieceIdentifier PieceIdentifier { get; init; }

    public required Point Position { get; init; }
    public required int InitialStateId { get; init; }
    public required GadgetRenderMode GadgetRenderMode { get; init; }

    public required Orientation Orientation { get; init; }
    public required FacingDirection FacingDirection { get; init; }

    public required HatchGadgetStateInstanceData[] GadgetStates { get; init; }

    public required int HatchGroupId { get; init; }
    public required int TribeId { get; init; }
    public required uint RawStateData { get; init; }
    public required int NumberOfLemmingsToRelease { get; init; }
}
