using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public unsafe readonly struct PointerWrapper<T>
    where T : unmanaged
{
    private readonly void* _pointer;

    public PointerWrapper(void* pointer) => _pointer = pointer;
    public PointerWrapper(nint handle) => _pointer = (void*)handle;

    public ref T Value => ref Unsafe.AsRef<T>(_pointer);
}
