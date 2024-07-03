using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public static class LevelReadWriteHelpers
{
    public const int PositionOffset = 512;
    private const int FlipBitShift = 2;

    #region String Data Read/Write Consts

    public static ReadOnlySpan<byte> StringDataSectionIdentifier => [0x26, 0x44];

    #endregion

    #region Level Data Read/Write Consts

    public static ReadOnlySpan<byte> LevelDataSectionIdentifier => [0x79, 0xA6];

    public const byte NoBackgroundSpecified = 0x00;
    public const byte SolidBackgroundColor = 0x01;
    public const byte BackgroundImageSpecified = 0x02;

    public const int UnspecifiedLevelStartValue = 5000;

    #endregion

    #region Hatch Group Data Read/Write Consts

    public static ReadOnlySpan<byte> HatchGroupDataSectionIdentifier => [0x90, 0xD2];

    #endregion

    #region Level Objectives Data Read/Write Consts

    public static ReadOnlySpan<byte> LevelObjectivesDataSectionIdentifier => [0xBE, 0xF4];

    public const int NumberOfBytesForMainLevelObjectiveData = 7;
    public const int NumberOfBytesPerSkillSetDatum = 3;
    public const int NumberOfBytesPerRequirementsDatum = 4;

    #endregion

    #region Pre-placed Lemming Data Read/Write Consts

    public static ReadOnlySpan<byte> PrePlacedLemmingDataSectionIdentifier => [0xFE, 0x77];

    #endregion

    #region Terrain Data Read/Write Consts

    public static ReadOnlySpan<byte> TerrainDataSectionIdentifier => [0x60, 0xBB];

    public const int EraseBitShift = 0;
    public const int NoOverwriteBitShift = 1;
    public const int TintBitShift = 2;
    public const int ResizeBitShift = 3;

    public const int NumberOfBytesForMainTerrainData = 9;

    #endregion

    #region Terrain Group Data Read/Write Consts

    public static ReadOnlySpan<byte> TerrainGroupDataSectionIdentifier => [0x7C, 0x5C];

    #endregion

    #region Gadget Data Read/Write Consts

    public static ReadOnlySpan<byte> GadgetDataSectionIdentifier => [0x3D, 0x98];

    #endregion

    public static void ReaderAssert(bool condition, string details)
    {
        if (condition)
            return;

        throw new LevelReadingException($"Error occurred when reading level file. Details: [{details}]");
    }

    public static byte GetOrientationByte(Orientation orientation, FacingDirection facingDirection)
    {
        return GetOrientationByte(orientation.RotNum, facingDirection == FacingDirection.LeftInstance);
    }

    public static byte GetOrientationByte(int rotNum, bool flip)
    {
        var orientationBits = (rotNum & 3) |
                              (flip ? 1 << FlipBitShift : 0);

        return (byte)orientationBits;
    }

    public static (Orientation, FacingDirection) DecipherOrientations(byte b)
    {
        var orientation = Orientation.AllItems[b & 3];
        var facingDirection = FacingDirection.AllItems[(b >> FlipBitShift) & 1];

        return (orientation, facingDirection);
    }
}