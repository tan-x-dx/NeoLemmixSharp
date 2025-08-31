using NeoLemmixSharp.Common;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.LogicGateGadget;

[DebuggerDisplay("Logic Gate - {GadgetType} - {StyleIdentifier}:{PieceIdentifier}")]
public sealed class LogicGateGadgetArchetypeData : IGadgetArchetypeData
{
    public GadgetType GadgetType => GadgetType.LogicGate;
    public required LogicGateGadgetType LogicGateGadgetType { get; init; }
    public required StyleIdentifier StyleIdentifier { get; init; }
    public required PieceIdentifier PieceIdentifier { get; init; }
    public required GadgetName GadgetName { get; init; }
    public required Size BaseSpriteSize { get; init; }
    public required LogicGateStateArchetypeData[] GadgetStates { get; init; }
}
