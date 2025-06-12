using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

[DebuggerDisplay("{StyleIdentifier}:{PieceIdentifier}")]
public sealed class GadgetArchetypeData
{
    internal BitArrayDictionary<GadgetArchetypeMiscDataTypeHasher, BitBuffer32, GadgetArchetypeMiscDataType, int> MiscData { get; } = GadgetArchetypeMiscDataTypeHasher.CreateBitArrayDictionary<int>();

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

    public required GadgetStateArchetypeData[] AllGadgetStateData { get; init; }

    internal void AddMiscData(GadgetArchetypeMiscDataType miscDataType, int value) => MiscData.Add(miscDataType, value);
    public bool HasMiscData(GadgetArchetypeMiscDataType miscDataType) => MiscData.ContainsKey(miscDataType);
    public int GetMiscData(GadgetArchetypeMiscDataType miscDataType) => MiscData[miscDataType];
    public bool TryGetMiscData(GadgetArchetypeMiscDataType miscDataType, out int value) => MiscData.TryGetValue(miscDataType, out value);

    internal GadgetArchetypeData()
    {
    }
}
