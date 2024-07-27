using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Collections;

public sealed class PerfectEnumHasher<TEnum> : IPerfectHasher<TEnum>
    where TEnum : struct, Enum
{
    private static readonly PerfectEnumHasher<TEnum> Instance = new();
    public static SimpleSet<TEnum> CreateSimpleSet() => new(Instance, false);

    public int NumberOfItems { get; } = Enum.GetValues<TEnum>().Length;

    private PerfectEnumHasher()
    {
    }

    public int Hash(TEnum item) => Unsafe.As<TEnum, int>(ref item);

    public TEnum UnHash(int index) => Unsafe.As<int, TEnum>(ref index);
}