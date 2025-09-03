using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.IO.Data.Level.Gadget.LogicGateGadget;

public sealed class LogicGateGadgetInstanceData : IGadgetInstanceData
{
    public required GadgetType GadgetType { get; init; }
    public required GadgetIdentifier Identifier { get; init; }
    public required GadgetName OverrideName { get; init; }
    public required StyleIdentifier StyleIdentifier { get; init; }
    public required PieceIdentifier PieceIdentifier { get; init; }
    public required Point Position { get; init; }
    public required int InitialStateId { get; init; }
    public required GadgetRenderMode GadgetRenderMode { get; init; }
    public required Orientation Orientation { get; init; }
    public required FacingDirection FacingDirection { get; init; }

    public required int NumberOfInputs { get; init; }
}
