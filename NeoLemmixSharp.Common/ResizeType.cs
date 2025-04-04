using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common;

[Flags]
public enum ResizeType
{
    None = 0,
    ResizeHorizontal = 1 << ResizeTypeHelpers.HorizontalShift,
    ResizeVertical = 1 << ResizeTypeHelpers.VerticalShift,
    ResizeBoth = ResizeHorizontal | ResizeVertical,
}

public static class ResizeTypeHelpers
{
    public const int HorizontalShift = 0;
    public const int VerticalShift = 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CanResizeHorizontally(this ResizeType type)
    {
        return (type & ResizeType.ResizeHorizontal) != ResizeType.None;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CanResizeVertically(this ResizeType type)
    {
        return (type & ResizeType.ResizeVertical) != ResizeType.None;
    }

    public static ResizeType SwapComponents(this ResizeType resizeType)
    {
        var intData = (int)resizeType;
        var b0 = intData & (1 << HorizontalShift);
        var b1 = intData & (1 << VerticalShift);

        b0 <<= 1;
        b1 >>>= 1;
        return (ResizeType)(b0 | b1);
    }
}