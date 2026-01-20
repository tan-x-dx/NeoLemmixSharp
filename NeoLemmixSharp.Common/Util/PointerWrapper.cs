using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public readonly unsafe struct PointerWrapper : IPointerData<PointerWrapper>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointerWrapper Create(nint dataRef) => new(dataRef);

    public static int SizeInBytes => sizeof(int);

    private readonly void* _pointer;

    public PointerWrapper(void* pointer) => _pointer = pointer;
    public PointerWrapper(nint handle) => _pointer = (void*)handle;

    public ref int IntValue => ref Unsafe.AsRef<int>(_pointer);
    public ref uint UintValue => ref Unsafe.AsRef<uint>(_pointer);
    public ref bool BoolValue => ref Unsafe.AsRef<bool>(_pointer);
}
