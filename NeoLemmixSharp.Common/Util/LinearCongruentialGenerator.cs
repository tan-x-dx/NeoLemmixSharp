using System.Collections;

namespace NeoLemmixSharp.Common.Util;

public struct LinearCongruentialGenerator<TLcgModulus> : IEnumerable<int>, IEnumerator<int>
    where TLcgModulus : unmanaged, ILcgModulus
{
    private readonly int _multiplier;
    private readonly int _increment;
    private int _x;

    public readonly int Current => _x;

    public LinearCongruentialGenerator(int multiplier, int increment)
    {
        _multiplier = multiplier;
        _increment = increment;
        _x = 0;
    }

    public bool MoveNext()
    {
        var result = _x;

        result *= _multiplier;
        result += _increment;
        _x = TLcgModulus.Modulo(result);

        return true;
    }

    public void Reset() => _x = 0;

    readonly object IEnumerator.Current => Current;
    public readonly LinearCongruentialGenerator<TLcgModulus> GetEnumerator() => this;
    readonly IEnumerator<int> IEnumerable<int>.GetEnumerator() => this;
    readonly IEnumerator IEnumerable.GetEnumerator() => this;
    readonly void IDisposable.Dispose() { }
}

public interface ILcgModulus
{
    static abstract int Modulo(int n);
}

public readonly struct LcgModulo65536 : ILcgModulus
{
    private const int BitMask = (1 << 16) - 1;

    public static int Modulo(int n) => n & BitMask;
}
