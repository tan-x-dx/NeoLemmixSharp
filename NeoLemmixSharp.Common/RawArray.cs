using NeoLemmixSharp.Common.Util;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common;

public readonly struct RawArray : IDisposable
{
    public readonly nint Handle;
    public readonly int Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RawArray(nint handle, int length)
    {
        Handle = handle;
        Length = length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RawArray(int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        Handle = Marshal.AllocHGlobal(length);
        Length = length;
    }

    public static void DoubleBufferSize(ref RawArray rawArray)
    {
        var newBufferLength = rawArray.Length * 2;
        nint newHandle = Marshal.ReAllocHGlobal(rawArray.Handle, newBufferLength);
        rawArray = new RawArray(newHandle, newBufferLength);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe Span<byte> AsSpan() => Helpers.CreateSpan<byte>((void*)Handle, Length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ReadOnlySpan<byte> AsReadOnlySpan() => Helpers.CreateReadOnlySpan<byte>((void*)Handle, Length);

    public void Dispose()
    {
        if (Handle != nint.Zero)
            Marshal.FreeHGlobal(Handle);
    }
}
