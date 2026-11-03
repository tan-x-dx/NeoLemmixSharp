using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public struct PackedBytes32
{
    private uint _bits;

    public byte Byte0
    {
        readonly get => GetByte(0);
        set => SetByte(0, value);
    }
    public byte Byte1
    {
        readonly get => GetByte(1);
        set => SetByte(1, value);
    }
    public byte Byte2
    {
        readonly get => GetByte(2);
        set => SetByte(2, value);
    }
    public byte Byte3
    {
        readonly get => GetByte(3);
        set => SetByte(3, value);
    }

    public ushort Ushort0
    {
        readonly get => GetUshort(0);
        set => SetUshort(0, value);
    }
    public ushort Ushort1
    {
        readonly get => GetUshort(1);
        set => SetUshort(1, value);
    }

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
    private readonly byte GetByte(int byteIndex)
    {
        const int ByteBitSize = 8;

        byteIndex *= ByteBitSize;
        uint result = _bits;
        result >>>= byteIndex;
        return (byte)result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetByte(int byteIndex, int value)
    {
        const int ByteBitSize = 8;
        const uint ByteBitMask = (1U << ByteBitSize) - 1;

        byteIndex *= ByteBitSize;

        uint valueU = (uint)value;
        valueU &= ByteBitMask;
        valueU <<= byteIndex;

        uint mask = ByteBitMask << byteIndex;
        uint bits = _bits;
        bits &= ~mask;

        bits |= valueU;
        _bits = bits;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly ushort GetUshort(int ushortIndex)
    {
        const int UshortBitSize = 16;

        ushortIndex *= UshortBitSize;
        uint result = _bits;
        result >>>= ushortIndex;
        return (ushort)result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetUshort(int ushortIndex, int value)
    {
        const int UshortBitSize = 16;
        const uint ByteBitMask = (1U << UshortBitSize) - 1;

        ushortIndex *= UshortBitSize;

        uint valueU = (uint)value;
        valueU &= ByteBitMask;
        valueU <<= ushortIndex;

        uint mask = ByteBitMask << ushortIndex;
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

    public byte Byte0
    {
        readonly get => GetByte(0);
        set => SetByte(0, value);
    }
    public byte Byte1
    {
        readonly get => GetByte(1);
        set => SetByte(1, value);
    }
    public byte Byte2
    {
        readonly get => GetByte(2);
        set => SetByte(2, value);
    }
    public byte Byte3
    {
        readonly get => GetByte(3);
        set => SetByte(3, value);
    }
    public byte Byte4
    {
        readonly get => GetByte(4);
        set => SetByte(4, value);
    }
    public byte Byte5
    {
        readonly get => GetByte(5);
        set => SetByte(5, value);
    }
    public byte Byte6
    {
        readonly get => GetByte(6);
        set => SetByte(6, value);
    }
    public byte Byte7
    {
        readonly get => GetByte(7);
        set => SetByte(7, value);
    }

    public ushort Ushort0
    {
        readonly get => GetUshort(0);
        set => SetUshort(0, value);
    }
    public ushort Ushort1
    {
        readonly get => GetUshort(1);
        set => SetUshort(1, value);
    }
    public ushort Ushort2
    {
        readonly get => GetUshort(2);
        set => SetUshort(2, value);
    }
    public ushort Ushort3
    {
        readonly get => GetUshort(3);
        set => SetUshort(3, value);
    }

    public uint Uint0
    {
        readonly get => GetUint(0);
        set => SetUint(0, value);
    }
    public uint Uint1
    {
        readonly get => GetUint(1);
        set => SetUint(1, value);
    }

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
    private readonly byte GetByte(int byteIndex)
    {
        const int ByteBitSize = 8;

        byteIndex *= ByteBitSize;
        ulong result = _bits;
        result >>>= byteIndex;
        return (byte)result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetByte(int byteIndex, int value)
    {
        const int ByteBitSize = 8;
        const ulong ByteBitMask = (1UL << ByteBitSize) - 1UL;

        byteIndex *= ByteBitSize;

        ulong valueUL = (uint)value;
        valueUL &= ByteBitMask;
        valueUL <<= byteIndex;

        ulong bits = _bits;
        ulong mask = ByteBitMask << byteIndex;
        bits &= ~mask;

        bits |= valueUL;
        _bits = bits;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly ushort GetUshort(int ushortIndex)
    {
        const int UshortBitSize = 16;

        ushortIndex *= UshortBitSize;
        ulong result = _bits;
        result >>>= ushortIndex;
        return (ushort)result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetUshort(int ushortIndex, int value)
    {
        const int UshortBitSize = 16;
        const ulong UshortBitMask = (1UL << UshortBitSize) - 1UL;

        ushortIndex *= UshortBitSize;

        ulong valueU = (uint)value;
        valueU &= UshortBitMask;
        valueU <<= ushortIndex;

        ulong mask = UshortBitMask << ushortIndex;
        ulong bits = _bits;
        bits &= ~mask;

        bits |= valueU;
        _bits = bits;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly uint GetUint(int uintIndex)
    {
        const int UintBitSize = 32;

        uintIndex *= UintBitSize;
        ulong result = _bits;
        result >>>= uintIndex;
        return (uint)result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetUint(int uintIndex, uint value)
    {
        const int UintBitSize = 32;
        const ulong UintBitMask = (1UL << UintBitSize) - 1UL;

        uintIndex *= UintBitSize;

        ulong valueU = value;
        valueU &= UintBitMask;
        valueU <<= uintIndex;

        ulong mask = UintBitMask << uintIndex;
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
