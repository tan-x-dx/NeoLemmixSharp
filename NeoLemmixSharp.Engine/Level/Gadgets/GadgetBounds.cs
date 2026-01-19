using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public unsafe readonly struct GadgetBounds : IRectangularBounds
{
    public const int GadgetBoundsDataSize = RectangularRegion.RectangularRegionSize;

    private readonly RectangularRegion* _data;

    public GadgetBounds(void* pointer) => _data = (RectangularRegion*)pointer;
    public GadgetBounds(nint pointerHandle) => _data = (RectangularRegion*)pointerHandle;

    public ref int X => ref Unsafe.AsRef<int>(&_data->X);
    public ref int Y => ref Unsafe.AsRef<int>(&_data->Y);
    public ref int Width => ref Unsafe.AsRef<int>(&_data->W);
    public ref int Height => ref Unsafe.AsRef<int>(&_data->H);

    public ref Point Position => ref Unsafe.AsRef<Point>(&_data->X);
    public Size Size => new(&_data->Size);

    public RectangularRegion CurrentBounds => new(_data);
}
