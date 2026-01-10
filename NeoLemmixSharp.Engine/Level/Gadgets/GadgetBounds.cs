using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

[StructLayout(LayoutKind.Explicit, Size = GadgetBoundsDataSize)]
public struct GadgetBoundsData
{
    public const int GadgetBoundsDataSize = 4 * sizeof(int);

    [FieldOffset(0 * sizeof(int))] public int X;
    [FieldOffset(1 * sizeof(int))] public int Y;

    [FieldOffset(2 * sizeof(int))] public int Width;
    [FieldOffset(3 * sizeof(int))] public int Height;
}

public unsafe sealed class GadgetBounds : IRectangularBounds
{
    private readonly GadgetBoundsData* _pointer;

    public GadgetBounds(void* pointer) => _pointer = (GadgetBoundsData*)pointer;
    public GadgetBounds(nint pointerHandle) => _pointer = (GadgetBoundsData*)pointerHandle;

    public ref int X => ref Unsafe.AsRef<int>(&_pointer->X);
    public ref int Y => ref Unsafe.AsRef<int>(&_pointer->Y);
    public ref int Width => ref Unsafe.AsRef<int>(&_pointer->Width);
    public ref int Height => ref Unsafe.AsRef<int>(&_pointer->Height);

    public ref Point Position => ref Unsafe.AsRef<Point>(&_pointer->X);
    public Size Size => new(&_pointer->Width);

    public RectangularRegion CurrentBounds => new(_pointer);
}
