using NeoLemmixSharp.Common;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

[DebuggerDisplay("{StyleIdentifier}:{PieceIdentifier}")]
public sealed class GadgetArchetypeData
{
    private readonly GadgetArchetypeMiscDictionary _miscData = GadgetArchetypeMiscDataTypeHasher.CreateBitArrayDictionary<int>();

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

    internal void AddMiscData(GadgetArchetypeMiscDataType miscDataType, int value) => _miscData.Add(miscDataType, value);
    public bool HasMiscData(GadgetArchetypeMiscDataType miscDataType) => _miscData.ContainsKey(miscDataType);
    public int GetMiscData(GadgetArchetypeMiscDataType miscDataType) => _miscData[miscDataType];
    public bool TryGetMiscData(GadgetArchetypeMiscDataType miscDataType, out int value) => _miscData.TryGetValue(miscDataType, out value);

    internal GadgetArchetypeData()
    {
    }
}
