using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.LevelIo.Data.Gadgets;

public enum GadgetProperty
{
    HatchGroupId,
    TeamId,
    SkillId,
    Width,
    Height,
    RawLemmingState,
    Count,
    InitialAnimationFrame,
    LogicGateType,
    IsFastForwards
}

public static class GadgetPropertyHelpers
{
    private const int NumberOfEnumValues = 9;

    public static GadgetProperty GetEnumValue(int rawValue) => Helpers.GetEnumValue<GadgetProperty>(rawValue, NumberOfEnumValues);
}

public readonly struct GadgetPropertyHasher : IPerfectHasher<GadgetProperty>, IBitBufferCreator<BitBuffer32>
{
    public int NumberOfItems => 10;

    [Pure]
    public int Hash(GadgetProperty item) => (int)item;
    [Pure]
    public GadgetProperty UnHash(int index) => (GadgetProperty)index;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArraySet<GadgetPropertyHasher, BitBuffer32, GadgetProperty> CreateBitArraySet(bool fullSet = false) => new(new GadgetPropertyHasher(), fullSet);
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArrayDictionary<GadgetPropertyHasher, BitBuffer32, GadgetProperty, TValue> CreateBitArrayDictionary<TValue>() => new(new GadgetPropertyHasher());

    public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();
}