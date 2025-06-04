using NeoLemmixSharp.Common;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

[DebuggerDisplay("{StyleIdentifier}:{PieceIdentifier}")]
public sealed class GadgetArchetypeData
{
    public required StyleIdentifier StyleIdentifier { get; init; }
    public required PieceIdentifier PieceIdentifier { get; init; }
    public required string GadgetName { get; init; }

    public required GadgetType GadgetType { get; init; }
    public required ResizeType ResizeType { get; init; }

    public required Size BaseSpriteSize { get; init; }
    /// <summary>
    /// Denotes the dimensions of the middle/middle nine-slice block
    /// </summary>
    public required RectangularRegion NineSliceData { get; init; }
    public required int NumberOfLayers { get; init; }
    public required int MaxNumberOfFrames { get; init; }

    public required GadgetStateArchetypeData[] AllGadgetStateData { get; init; }

    internal GadgetArchetypeData()
    {
    }
}
