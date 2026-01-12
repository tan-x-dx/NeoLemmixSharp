using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public unsafe readonly struct PointerWrapper
{
    private readonly void* _pointer;

    public PointerWrapper(void* pointer) => _pointer = pointer;
    public PointerWrapper(nint handle) => _pointer = (void*)handle;

    public ref int IntValue => ref Unsafe.AsRef<int>(_pointer);
    public ref uint UintValue => ref Unsafe.AsRef<uint>(_pointer);
    public ref bool BoolValue => ref Unsafe.AsRef<bool>(_pointer);
}
