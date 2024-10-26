using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Collections;

public sealed class PerfectEnumHasher<TEnum> : IPerfectHasher<TEnum>
    where TEnum : struct, Enum
{
    private static readonly PerfectEnumHasher<TEnum> Instance = new();

    public static SimpleSet<PerfectEnumHasher<TEnum>, TEnum> CreateSimpleSet() => new(Instance, false);
    public static SimpleDictionary<PerfectEnumHasher<TEnum>, TEnum, TValue> CreateSimpleDictionary<TValue>() => new(Instance);

    public int NumberOfItems { get; } = Enum.GetNames<TEnum>().Length;

    private PerfectEnumHasher()
    {
    }

    public int Hash(TEnum item) => Unsafe.As<TEnum, int>(ref item);

    public TEnum UnHash(int index) => Unsafe.As<int, TEnum>(ref index);
}