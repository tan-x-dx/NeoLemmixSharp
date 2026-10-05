using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public struct PackedBytes
{
    private uint _bits;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => _bits = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetRawValue(uint value) => _bits = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool GetBit(int bitIndex)
    {
        var result = _bits;
        result >>>= bitIndex;
        return (result & 1) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBit(int bitIndex)
    {
        var mask = 1U << bitIndex;
        _bits |= mask;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBit(int bitIndex, bool bitValue)
    {
        var mask = 1U << bitIndex;
        var bits = _bits;
        bits &= ~mask;
        mask = bitValue ? 1U : 0U;
        mask <<= bitIndex;
        bits |= mask;
        _bits = bits;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearBit(int bitIndex)
    {
        var mask = ~(1U << bitIndex);
        _bits &= mask;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int GetByte(int byteIndex)
    {
        byteIndex &= 3;
        byteIndex *= 8;
        var result = _bits;
        result >>>= byteIndex;
        return (int)result & 0xff;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetByte(int byteIndex, int value)
    {
        byteIndex &= 3;
        byteIndex *= 8;
        var mask = 0xffU << byteIndex;
        var bits = _bits;
        bits &= ~mask;

        value &= 0xff;
        value <<= byteIndex;

        bits |= (uint)value;
        _bits = bits;
    }
}
