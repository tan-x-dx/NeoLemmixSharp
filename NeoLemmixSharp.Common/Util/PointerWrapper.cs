using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public readonly unsafe struct PointerWrapper : IPointerData<PointerWrapper>
{
    public const int PointerWrapperSize = sizeof(int);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointerWrapper Create(nint dataRef) => new(dataRef);

    public static int SizeInBytes => PointerWrapperSize;

    private readonly void* _pointer;

    public PointerWrapper(void* pointer) => _pointer = pointer;
    public PointerWrapper(nint handle) => _pointer = (void*)handle;

    public ref int IntValue => ref Unsafe.AsRef<int>(_pointer);
    public ref uint UintValue => ref Unsafe.AsRef<uint>(_pointer);
    public ref bool BoolValue => ref Unsafe.AsRef<bool>(_pointer);

    public static implicit operator ReadOnlyPointerWrapper(PointerWrapper pointer) => new(pointer._pointer);
}

public readonly unsafe struct ReadOnlyPointerWrapper : IPointerData<ReadOnlyPointerWrapper>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyPointerWrapper Create(nint dataRef) => new(dataRef);

    public static int SizeInBytes => PointerWrapper.PointerWrapperSize;

    private readonly void* _pointer;

    public ReadOnlyPointerWrapper(void* pointer) => _pointer = pointer;
    public ReadOnlyPointerWrapper(nint handle) => _pointer = (void*)handle;

    public int IntValue => *(int*)_pointer;
    public uint UintValue => *(uint*)_pointer;
    public bool BoolValue => *(bool*)_pointer;
}
