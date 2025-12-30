using NeoLemmixSharp.Common.Util;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common;

public readonly struct RawArray : IDisposable
{
    public readonly nint Handle;
    public readonly int Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RawArray(nint handle, int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);

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
