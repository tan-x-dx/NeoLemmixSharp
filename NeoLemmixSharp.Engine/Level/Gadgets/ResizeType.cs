namespace NeoLemmixSharp.Engine.Level.Gadgets;

[Flags]
public enum ResizeType
{
    None = 0,
    ResizeHorizontal = 1 << 0,
    ResizeVertical = 1 << 1,
    ResizeBoth = ResizeHorizontal | ResizeVertical,
}

public static class ResizeTypeHelpers
{
    public static ResizeType SwapComponents(this ResizeType resizeType)
    {
        var intData = (int)resizeType;
        var b0 = intData & (1 << 0);
        var b1 = intData & (1 << 1);

        b0 <<= 1;
        b1 >>= 1;
        return (ResizeType)(b0 | b1);
    }
}