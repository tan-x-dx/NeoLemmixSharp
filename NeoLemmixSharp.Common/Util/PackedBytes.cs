using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public struct PackedBytes32
{
    private uint _bits;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => _bits = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly uint GetRawValue() => _bits;

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
    public readonly byte GetByte(int byteIndex)
    {
        byteIndex &= 3;
        byteIndex *= 8;
        uint result = _bits;
        result >>>= byteIndex;
        return (byte)result;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ushort GetUshort(int ushortIndex)
    {
        ushortIndex &= 1;
        ushortIndex *= 16;
        uint result = _bits;
        result >>>= ushortIndex;
        return (ushort)result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetUshort(int ushortIndex, int value)
    {
        ushortIndex &= 1;
        ushortIndex *= 16;

        uint valueU = (uint)value;
        valueU &= 0xffffU;
        valueU <<= ushortIndex;

        uint mask = 0xffffU << ushortIndex;
        uint bits = _bits;
        bits &= ~mask;

        bits |= valueU;
        _bits = bits;
    }

    public static implicit operator PackedBytes32(uint rawValue) => new() { _bits = rawValue };
    public static PackedBytes32 operator ~(PackedBytes32 value) => new() { _bits = ~value._bits };
    public static PackedBytes32 operator &(PackedBytes32 left, PackedBytes32 right) => new() { _bits = left._bits & right._bits };
    public static PackedBytes32 operator |(PackedBytes32 left, PackedBytes32 right) => new() { _bits = left._bits | right._bits };
    public static PackedBytes32 operator ^(PackedBytes32 left, PackedBytes32 right) => new() { _bits = left._bits ^ right._bits };
}

public struct PackedBytes64
{
    private ulong _bits;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ulong GetRawValue() => _bits;

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
    public readonly byte GetByte(int byteIndex)
    {
        byteIndex &= 7;
        byteIndex *= 8;
        ulong result = _bits;
        result >>>= byteIndex;
        return (byte)result;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ushort GetUshort(int ushortIndex)
    {
        ushortIndex &= 3;
        ushortIndex *= 16;
        ulong result = _bits;
        result >>>= ushortIndex;
        return (ushort)result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetUshort(int ushortIndex, int value)
    {
        ushortIndex &= 3;
        ushortIndex *= 16;

        ulong valueU = (ulong)(uint)value;
        valueU &= 0xffffU;
        valueU <<= ushortIndex;

        ulong mask = 0xffffU << ushortIndex;
        ulong bits = _bits;
        bits &= ~mask;

        bits |= valueU;
        _bits = bits;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly uint GetUint(int uintIndex)
    {
        uintIndex &= 1;
        uintIndex *= 32;
        ulong result = _bits;
        result >>>= uintIndex;
        return (uint)result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetUint(int uintIndex, int value)
    {
        uintIndex &= 1;
        uintIndex *= 32;

        ulong valueU = (ulong)(uint)value;
        valueU &= 0xffffffffU;
        valueU <<= uintIndex;

        ulong mask = 0xffffffffU << uintIndex;
        ulong bits = _bits;
        bits &= ~mask;

        bits |= valueU;
        _bits = bits;
    }

    public static implicit operator PackedBytes64(ulong rawValue) => new() { _bits = rawValue };
    public static PackedBytes64 operator ~(PackedBytes64 value) => new() { _bits = ~value._bits };
    public static PackedBytes64 operator &(PackedBytes64 left, PackedBytes64 right) => new() { _bits = left._bits & right._bits };
    public static PackedBytes64 operator |(PackedBytes64 left, PackedBytes64 right) => new() { _bits = left._bits | right._bits };
    public static PackedBytes64 operator ^(PackedBytes64 left, PackedBytes64 right) => new() { _bits = left._bits ^ right._bits };
}
