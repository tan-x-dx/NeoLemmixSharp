using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Terrain;

[Flags]
public enum PixelType : ushort
{
    Empty = 0,

    DownSolid = 1 << (EngineConstants.DownOrientationRotNum + PixelTypeHelpers.PixelTypeSolidShiftOffset),
    LeftSolid = 1 << (EngineConstants.LeftOrientationRotNum + PixelTypeHelpers.PixelTypeSolidShiftOffset),
    UpSolid = 1 << (EngineConstants.UpOrientationRotNum + PixelTypeHelpers.PixelTypeSolidShiftOffset),
    RightSolid = 1 << (EngineConstants.RightOrientationRotNum + PixelTypeHelpers.PixelTypeSolidShiftOffset),

    SolidToAllOrientations = DownSolid | LeftSolid | UpSolid | RightSolid,

    DownArrow = 1 << (EngineConstants.DownOrientationRotNum + PixelTypeHelpers.PixelTypeArrowShiftOffset),
    LeftArrow = 1 << (EngineConstants.LeftOrientationRotNum + PixelTypeHelpers.PixelTypeArrowShiftOffset),
    UpArrow = 1 << (EngineConstants.UpOrientationRotNum + PixelTypeHelpers.PixelTypeArrowShiftOffset),
    RightArrow = 1 << (EngineConstants.RightOrientationRotNum + PixelTypeHelpers.PixelTypeArrowShiftOffset),

    ArrowMask = DownArrow | LeftArrow | UpArrow | RightArrow,
    ArrowInverseMask = ushort.MaxValue ^ ArrowMask,

    TerrainDataMask = SolidToAllOrientations | ArrowMask,
    TerrainDataInverseMask = ushort.MaxValue ^ TerrainDataMask,

    BlockerDown = 1 << (EngineConstants.DownOrientationRotNum + PixelTypeHelpers.PixelTypeBlockerShiftOffset),
    BlockerLeft = 1 << (EngineConstants.LeftOrientationRotNum + PixelTypeHelpers.PixelTypeBlockerShiftOffset),
    BlockerUp = 1 << (EngineConstants.UpOrientationRotNum + PixelTypeHelpers.PixelTypeBlockerShiftOffset),
    BlockerRight = 1 << (EngineConstants.RightOrientationRotNum + PixelTypeHelpers.PixelTypeBlockerShiftOffset),

    BlockerMask = BlockerDown | BlockerLeft | BlockerRight | BlockerUp,
    BlockerInverseMask = ushort.MaxValue ^ BlockerMask,

    Steel = 1 << PixelTypeHelpers.PixelTypeSteelShift,
    Void = 1 << PixelTypeHelpers.PixelTypeVoidShift
}