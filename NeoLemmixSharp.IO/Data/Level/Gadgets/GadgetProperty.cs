using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Data.Level.Gadgets;

public enum GadgetProperty
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
    IsFastForwards
}

public readonly struct GadgetPropertyHasher : IPerfectHasher<GadgetProperty>, IBitBufferCreator<BitBuffer32>
{
    private const int NumberOfEnumValues = 10;

    public int NumberOfItems => NumberOfEnumValues;

    [Pure]
    public int Hash(GadgetProperty item) => (int)item;
    [Pure]
    public GadgetProperty UnHash(int index) => (GadgetProperty)index;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArraySet<GadgetPropertyHasher, BitBuffer32, GadgetProperty> CreateBitArraySet() => new(new GadgetPropertyHasher());
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArrayDictionary<GadgetPropertyHasher, BitBuffer32, GadgetProperty, TValue> CreateBitArrayDictionary<TValue>() => new(new GadgetPropertyHasher());

    public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();

    public static GadgetProperty GetEnumValue(uint rawValue) => Helpers.GetEnumValue<GadgetProperty>(rawValue, NumberOfEnumValues);
}