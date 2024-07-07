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

    #region Level Text Data Read/Write Consts

    public static ReadOnlySpan<byte> LevelTextDataSectionIdentifier => [0x43, 0xAA];

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

    public const int TerrainDataEraseBitShift = 0;
    public const int TerrainDataNoOverwriteBitShift = 1;
    public const int TerrainDataTintBitShift = 2;
    public const int TerrainDataResizeWidthBitShift = 3;
    public const int TerrainDataResizeHeightBitShift = 4;

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

    public static (Orientation, FacingDirection) DecipherOrientationByte(int b)
    {
        var orientation = Orientation.AllItems[b & 3];
        var facingDirection = FacingDirection.AllItems[(b >> FlipBitShift) & 1];

        return (orientation, facingDirection);
    }

    public static void DecipherTerrainDataMiscByte(byte b, out DecipheredTerrainDataMisc decipheredTerrainDataMisc)
    {
        var erase = ((b >> TerrainDataEraseBitShift) & 1) != 0;
        var noOverwrite = ((b >> TerrainDataNoOverwriteBitShift) & 1) != 0;
        var hasTintSpecified = ((b >> TerrainDataTintBitShift) & 1) != 0;
        var hasWidthSpecified = ((b >> TerrainDataResizeWidthBitShift) & 1) != 0;
        var hasHeightSpecified = ((b >> TerrainDataResizeHeightBitShift) & 1) != 0;

        decipheredTerrainDataMisc = new DecipheredTerrainDataMisc(
            erase,
            noOverwrite,
            hasTintSpecified,
            hasWidthSpecified,
            hasHeightSpecified);
    }

    public readonly ref struct DecipheredTerrainDataMisc
    {
        public readonly bool Erase;
        public readonly bool NoOverwrite;
        public readonly bool HasTintSpecified;
        public readonly bool HasWidthSpecified;
        public readonly bool HasHeightSpecified;

        public DecipheredTerrainDataMisc(bool erase, bool noOverwrite, bool hasTintSpecified, bool hasWidthSpecified, bool hasHeightSpecified)
        {
            Erase = erase;
            NoOverwrite = noOverwrite;
            HasTintSpecified = hasTintSpecified;
            HasWidthSpecified = hasWidthSpecified;
            HasHeightSpecified = hasHeightSpecified;
        }
    }
}