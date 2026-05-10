namespace NeoLemmixSharp.Common;

public readonly struct NineSliceData
{
    public readonly ushort Left;
    public readonly ushort Right;
    public readonly ushort Top;
    public readonly ushort Bottom;

    public NineSliceData(ushort left, ushort right, ushort top, ushort bottom)
    {
        Left = left;
        Right = right;
        Top = top;
        Bottom = bottom;
    }
}
