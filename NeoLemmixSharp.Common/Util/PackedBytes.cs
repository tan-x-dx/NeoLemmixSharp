using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public struct PackedBytes32
{
    private uint _bits;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => _bits = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetRawValue(uint value) => _bits = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool GetBit(int bitIndex)
    {
        uint result = _bits;
        result >>>= bitIndex;
        return (result & 1) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBit(int bitIndex)
    {
        uint mask = 1U << bitIndex;
        _bits |= mask;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBit(int bitIndex, bool bitValue)
    {
        uint mask = 1U << bitIndex;
        uint bits = _bits;
        bits &= ~mask;
        mask = bitValue ? 1U : 0U;
        mask <<= bitIndex;
        bits |= mask;
        _bits = bits;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearBit(int bitIndex)
    {
        uint mask = ~(1U << bitIndex);
        _bits &= mask;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int GetByte(int byteIndex)
    {
        byteIndex &= 3;
        byteIndex *= 8;
        uint result = _bits;
        result >>>= byteIndex;
        return (int)result & 0xff;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetByte(int byteIndex, int value)
    {
        byteIndex &= 3;
        byteIndex *= 8;

        uint valueU = (uint)value;
        valueU &= 0xffU;
        valueU <<= byteIndex;

        uint mask = 0xffU << byteIndex;
        uint bits = _bits;
        bits &= ~mask;

        bits |= valueU;
        _bits = bits;
    }
}

public struct PackedBytes64
{
    private ulong _bits;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => _bits = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetRawValue(ulong value) => _bits = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool GetBit(int bitIndex)
    {
        ulong result = _bits;
        result >>>= bitIndex;
        return (result & 1UL) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBit(int bitIndex)
    {
        var mask = 1UL << bitIndex;
        _bits |= mask;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBit(int bitIndex, bool bitValue)
    {
        ulong mask = 1UL << bitIndex;
        ulong bits = _bits;
        bits &= ~mask;
        mask = bitValue ? 1UL : 0UL;
        mask <<= bitIndex;
        bits |= mask;
        _bits = bits;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearBit(int bitIndex)
    {
        ulong mask = ~(1UL << bitIndex);
        _bits &= mask;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int GetByte(int byteIndex)
    {
        byteIndex &= 7;
        byteIndex *= 8;
        ulong result = _bits;
        result >>>= byteIndex;
        return (int)(result & 0xffUL);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetByte(int byteIndex, int value)
    {
        byteIndex &= 7;
        byteIndex *= 8;

        ulong valueUL = (ulong)(uint)value;
        valueUL &= 0xffUL;
        valueUL <<= byteIndex;

        ulong bits = _bits;
        ulong mask = 0xffUL << byteIndex;
        bits &= ~mask;

        bits |= valueUL;
        _bits = bits;
    }
}
