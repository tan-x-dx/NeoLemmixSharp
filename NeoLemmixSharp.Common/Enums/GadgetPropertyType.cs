using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Enums;

public enum GadgetPropertyType
{
    HatchGroupId,
    TribeId,
    SkillId,
    Width,
    Height,
    RawLemmingState,
    Count,
    InitialAnimationFrame,
    LogicGateType,
    IsFastForwards,
    NumberOfInputs
}

public readonly struct GadgetPropertyTypeHasher : IEnumIdentifierHelper<BitBuffer32, GadgetPropertyType>
{
    private const int NumberOfEnumValues = 11;

    public int NumberOfItems => NumberOfEnumValues;

    [Pure]
    public int Hash(GadgetPropertyType item) => (int)item;
    [Pure]
    public GadgetPropertyType UnHash(int index) => (GadgetPropertyType)index;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArraySet<GadgetPropertyTypeHasher, BitBuffer32, GadgetPropertyType> CreateBitArraySet() => new(new GadgetPropertyTypeHasher());
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArrayDictionary<GadgetPropertyTypeHasher, BitBuffer32, GadgetPropertyType, TValue> CreateBitArrayDictionary<TValue>() => new(new GadgetPropertyTypeHasher());

    public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();

    public static GadgetPropertyType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<GadgetPropertyType>(rawValue, NumberOfEnumValues);
}
