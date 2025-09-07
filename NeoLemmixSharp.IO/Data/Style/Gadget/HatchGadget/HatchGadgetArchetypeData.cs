using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.HatchGadget;

[DebuggerDisplay("Hatch - {StyleIdentifier}:{PieceIdentifier}")]
public sealed class HatchGadgetArchetypeData : IGadgetArchetypeData
{
    public required StyleIdentifier StyleIdentifier { get; init; }
    public required PieceIdentifier PieceIdentifier { get; init; }
    public required GadgetName GadgetName { get; init; }

    public GadgetType GadgetType => GadgetType.HatchGadget;
    public required Size BaseSpriteSize { get; init; }

    public required Point SpawnOffset { get; init; }
    public required HatchGadgetStateArchetypeData[] GadgetStates { get; init; }
}
