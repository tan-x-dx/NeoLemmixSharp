using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;

[DebuggerDisplay("HitBox - {StyleIdentifier}:{PieceIdentifier}")]
public sealed class HitBoxGadgetArchetypeData : IGadgetArchetypeData
{
    private readonly BitArrayDictionary<GadgetArchetypeMiscDataTypeHasher, BitBuffer32, GadgetArchetypeMiscDataType, int> _miscData = GadgetArchetypeMiscDataTypeHasher.CreateBitArrayDictionary<int>();

    public required StyleIdentifier StyleIdentifier { get; init; }
    public required PieceIdentifier PieceIdentifier { get; init; }
    public required GadgetName GadgetName { get; init; }

    public GadgetType GadgetType => GadgetType.HitBoxGadget;
    public required ResizeType ResizeType { get; init; }

    public required Size BaseSpriteSize { get; init; }
    public required RectangularRegion NineSliceData { get; init; }

    public required HitBoxGadgetStateArchetypeData[] GadgetStates { get; init; }

    internal void AddMiscData(GadgetArchetypeMiscDataType miscDataType, int value) => _miscData.Add(miscDataType, value);
    public bool HasMiscData(GadgetArchetypeMiscDataType miscDataType) => _miscData.ContainsKey(miscDataType);
    public int GetMiscData(GadgetArchetypeMiscDataType miscDataType) => _miscData[miscDataType];
    public bool TryGetMiscData(GadgetArchetypeMiscDataType miscDataType, out int value) => _miscData.TryGetValue(miscDataType, out value);
}
