namespace NeoLemmixSharp.Common.Util;

[Flags]
public enum PixelType : ushort
{
    Empty = 0,

    DownSolid = 1 << 0,
    LeftSolid = 1 << 1,
    UpSolid = 1 << 2,
    RightSolid = 1 << 3,

    SolidToAllOrientations = DownSolid | LeftSolid | UpSolid | RightSolid,

    DownArrow = 1 << 4,
    LeftArrow = 1 << 5,
    UpArrow = 1 << 6,
    RightArrow = 1 << 7,

    BlockerDown = 1 << 8,
    BlockerLeft = 1 << 9,
    BlockerUp = 1 << 10,
    BlockerRight = 1 << 11,

    BlockerMask = BlockerDown | BlockerLeft | BlockerRight | BlockerUp,
    ClearBlockerMask = ushort.MaxValue ^ BlockerMask,

    Steel = 1 << 14,
    Void = 1 << 15
}