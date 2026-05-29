using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public readonly unsafe struct GadgetBounds : IPointerData<GadgetBounds>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GadgetBounds Create(nint dataRef) => new(dataRef);

    public static int SizeInBytes => GadgetBoundsDataSize;

    private const int GadgetBoundsDataSize = 4 * sizeof(int);

    [StructLayout(LayoutKind.Explicit, Size = GadgetBoundsDataSize)]
    private struct GadgetBoundsRaw
    {
        [FieldOffset(0 * sizeof(int))] public int X;
        [FieldOffset(1 * sizeof(int))] public int Y;
        [FieldOffset(2 * sizeof(int))] public int Width;
        [FieldOffset(3 * sizeof(int))] public int Height;

        [FieldOffset(0 * sizeof(int))] public Point Position;
    }

    private readonly GadgetBoundsRaw* _data;

    private GadgetBounds(nint pointerHandle) => _data = (GadgetBoundsRaw*)pointerHandle;

    public ref int X => ref Unsafe.AsRef<int>(&_data->X);
    public ref int Y => ref Unsafe.AsRef<int>(&_data->Y);
    public ref int Width => ref Unsafe.AsRef<int>(&_data->Width);
    public ref int Height => ref Unsafe.AsRef<int>(&_data->Height);

    public ref Point Position => ref Unsafe.AsRef<Point>(&_data->Position);
}
