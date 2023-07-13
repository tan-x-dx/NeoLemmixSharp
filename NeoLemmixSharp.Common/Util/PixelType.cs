namespace NeoLemmixSharp.Common.Util;

[Flags]
public enum PixelType : byte
{
    Empty = 0b0000_0000,
    Solid = 0b0000_0001,
    Steel = 0b0000_0010 | Solid,

    DownArrow = 0b0000_0100 | Solid,
    LeftArrow = 0b0000_1000 | Solid,
    UpArrow = 0b0001_0000 | Solid,
    RightArrow = 0b0010_0000 | Solid,

    Void = 0b1000_0000
}