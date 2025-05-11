using NeoLemmixSharp.Common;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

[DebuggerDisplay("{StyleName}:{PieceName}")]
public sealed class GadgetArchetypeData
{
    public required StyleIdentifier StyleName { get; init; }
    public required PieceIdentifier PieceName { get; init; }

    public required GadgetType GadgetType { get; init; }
    public required ResizeType ResizeType { get; init; }

    public required GadgetStateArchetypeData[] AllGadgetStateData { get; init; }
    public required SpriteArchetypeData SpriteData { get; init; }

    internal GadgetArchetypeData()
    {
    }
}
