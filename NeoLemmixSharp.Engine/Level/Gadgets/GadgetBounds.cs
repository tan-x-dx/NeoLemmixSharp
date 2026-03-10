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

    [StructLayout(LayoutKind.Sequential, Size = GadgetBoundsDataSize)]
    private struct GadgetBoundsRaw
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }

    private readonly GadgetBoundsRaw* _data;

    private GadgetBounds(nint pointerHandle) => _data = (GadgetBoundsRaw*)pointerHandle;

    public ref int X => ref Unsafe.AsRef<int>(&_data->X);
    public ref int Y => ref Unsafe.AsRef<int>(&_data->Y);
    public ref int Width => ref Unsafe.AsRef<int>(&_data->Width);
    public ref int Height => ref Unsafe.AsRef<int>(&_data->Height);

    public ref Point Position => ref Unsafe.AsRef<Point>(&_data->X);
}
