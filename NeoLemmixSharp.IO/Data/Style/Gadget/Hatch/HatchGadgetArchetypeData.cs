using NeoLemmixSharp.Common;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.Hatch;

[DebuggerDisplay("Hatch - {StyleIdentifier}:{PieceIdentifier}")]
public sealed class HatchGadgetArchetypeData : IGadgetArchetypeData
{
    public required StyleIdentifier StyleIdentifier { get; init; }
    public required PieceIdentifier PieceIdentifier { get; init; }
    public required string GadgetName { get; init; }

    public GadgetType GadgetType => GadgetType.HatchGadget;
    public required Size BaseSpriteSize { get; init; }

    public required Point SpawnOffset { get; init; }
    IGadgetStateArchetypeData[] IGadgetArchetypeData.GadgetStates => GadgetStates;
    public required HatchGadgetStateArchetypeData[] GadgetStates { get; init; }
}
