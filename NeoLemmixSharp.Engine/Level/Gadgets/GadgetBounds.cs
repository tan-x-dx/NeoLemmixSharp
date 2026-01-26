using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public readonly unsafe struct GadgetBounds : IPointerData<GadgetBounds>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GadgetBounds Create(nint dataRef) => new(dataRef);

    public static int SizeInBytes => GadgetBoundsDataSize;

    private const int GadgetBoundsDataSize = RectangularRegion.RectangularRegionSize;

    private readonly RectangularRegion* _data;

    private GadgetBounds(nint pointerHandle) => _data = (RectangularRegion*)pointerHandle;

    public ref int X => ref Unsafe.AsRef<int>(&_data->X);
    public ref int Y => ref Unsafe.AsRef<int>(&_data->Y);
    public ref int Width => ref Unsafe.AsRef<int>(&_data->W);
    public ref int Height => ref Unsafe.AsRef<int>(&_data->H);

    public ref Point Position => ref Unsafe.AsRef<Point>(&_data->Position);
}
