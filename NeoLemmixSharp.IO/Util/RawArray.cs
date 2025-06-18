using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.IO.Util;

public readonly unsafe struct RawArray : IDisposable
{
    public readonly nint Handle;
    public readonly int Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RawArray(nint handle, int length)
    {
        Handle = handle;
        Length = length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RawArray(int length)
    {
        Handle = Marshal.AllocHGlobal(length);
        Length = length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> AsSpan() => new((void*)Handle, Length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> AsReadOnlySpan() => new((void*)Handle, Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (Handle != nint.Zero)
            Marshal.FreeHGlobal(Handle);
    }
}
