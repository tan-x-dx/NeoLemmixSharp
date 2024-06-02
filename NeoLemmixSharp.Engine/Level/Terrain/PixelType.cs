namespace NeoLemmixSharp.Engine.Level.Terrain;

[Flags]
public enum PixelType : ushort
{
    Empty = 0,

    DownSolid = 1 << (LevelConstants.DownOrientationRotNum + PixelTypeHelpers.PixelTypeSolidShiftOffset),
    LeftSolid = 1 << (LevelConstants.LeftOrientationRotNum + PixelTypeHelpers.PixelTypeSolidShiftOffset),
    UpSolid = 1 << (LevelConstants.UpOrientationRotNum + PixelTypeHelpers.PixelTypeSolidShiftOffset),
    RightSolid = 1 << (LevelConstants.RightOrientationRotNum + PixelTypeHelpers.PixelTypeSolidShiftOffset),

    SolidToAllOrientations = DownSolid | LeftSolid | UpSolid | RightSolid,

    DownArrow = 1 << (LevelConstants.DownOrientationRotNum + PixelTypeHelpers.PixelTypeArrowShiftOffset),
    LeftArrow = 1 << (LevelConstants.LeftOrientationRotNum + PixelTypeHelpers.PixelTypeArrowShiftOffset),
    UpArrow = 1 << (LevelConstants.UpOrientationRotNum + PixelTypeHelpers.PixelTypeArrowShiftOffset),
    RightArrow = 1 << (LevelConstants.RightOrientationRotNum + PixelTypeHelpers.PixelTypeArrowShiftOffset),

    ArrowMask = DownArrow | LeftArrow | UpArrow | RightArrow,
    ArrowInverseMask = ushort.MaxValue ^ ArrowMask,

    TerrainDataMask = SolidToAllOrientations | ArrowMask,
    TerrainDataInverseMask = ushort.MaxValue ^ TerrainDataMask,

    BlockerDown = 1 << (LevelConstants.DownOrientationRotNum + PixelTypeHelpers.PixelTypeBlockerShiftOffset),
    BlockerLeft = 1 << (LevelConstants.LeftOrientationRotNum + PixelTypeHelpers.PixelTypeBlockerShiftOffset),
    BlockerUp = 1 << (LevelConstants.UpOrientationRotNum + PixelTypeHelpers.PixelTypeBlockerShiftOffset),
    BlockerRight = 1 << (LevelConstants.RightOrientationRotNum + PixelTypeHelpers.PixelTypeBlockerShiftOffset),

    BlockerMask = BlockerDown | BlockerLeft | BlockerRight | BlockerUp,
    BlockerInverseMask = ushort.MaxValue ^ BlockerMask,

    Steel = 1 << PixelTypeHelpers.PixelTypeSteelShift,
    Void = 1 << PixelTypeHelpers.PixelTypeVoidShift
}