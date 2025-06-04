using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public enum GadgetArchetypeMiscDataType
{
    SpawnPointOffset,
}

public readonly struct GadgetArchetypeMiscDataTypeHasher : IPerfectHasher<GadgetArchetypeMiscDataType>, IBitBufferCreator<BitBuffer32>
{
    private const int NumberOfEnumValues = 8;

    public int NumberOfItems => NumberOfEnumValues;

    public static GadgetArchetypeMiscDataType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<GadgetArchetypeMiscDataType>(rawValue, NumberOfEnumValues);

    [Pure]
    public int Hash(GadgetArchetypeMiscDataType item) => (int)item;
    [Pure]
    public GadgetArchetypeMiscDataType UnHash(int index) => (GadgetArchetypeMiscDataType)index;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArraySet<GadgetArchetypeMiscDataTypeHasher, BitBuffer32, GadgetArchetypeMiscDataType> CreateBitArraySet() => new(new GadgetArchetypeMiscDataTypeHasher());
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArrayDictionary<GadgetArchetypeMiscDataTypeHasher, BitBuffer32, GadgetArchetypeMiscDataType, TValue> CreateBitArrayDictionary<TValue>() => new(new GadgetArchetypeMiscDataTypeHasher());

    public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();
}
