using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public unsafe readonly struct PointerWrapper<T>(void* pointer)
    where T : unmanaged
{
    private readonly void* _pointer = pointer;

    public ref T Value => ref Unsafe.AsRef<T>(_pointer);
}
