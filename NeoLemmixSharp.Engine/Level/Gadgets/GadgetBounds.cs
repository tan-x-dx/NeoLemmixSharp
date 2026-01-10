using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public unsafe readonly struct GadgetBounds : IRectangularBounds
{
    public const int GadgetBoundsDataSize = 4 * sizeof(int);

    [StructLayout(LayoutKind.Explicit, Size = GadgetBoundsDataSize)]
    private struct GadgetBoundsData
    {
        [FieldOffset(0 * sizeof(int))] public int X;
        [FieldOffset(1 * sizeof(int))] public int Y;

        [FieldOffset(2 * sizeof(int))] public int Width;
        [FieldOffset(3 * sizeof(int))] public int Height;
    }

    private readonly GadgetBoundsData* _data;

    public GadgetBounds(void* pointer) => _data = (GadgetBoundsData*)pointer;
    public GadgetBounds(nint pointerHandle) => _data = (GadgetBoundsData*)pointerHandle;

    public ref int X => ref Unsafe.AsRef<int>(&_data->X);
    public ref int Y => ref Unsafe.AsRef<int>(&_data->Y);
    public ref int Width => ref Unsafe.AsRef<int>(&_data->Width);
    public ref int Height => ref Unsafe.AsRef<int>(&_data->Height);

    public ref Point Position => ref Unsafe.AsRef<Point>(&_data->X);
    public Size Size => new(&_data->Width);

    public RectangularRegion CurrentBounds => new(_data);
}
