using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public static class LevelReadWriteHelpers
{
    public const int PositionOffset = 512;

    public const int NumberOfBytesForSectionIdentifier = 2;

    public const int StringDataSectionIdentifierIndex = 0;
    public const int LevelMetadataSectionIdentifierIndex = 2;
    public const int LevelTextDataSectionIdentifierIndex = 4;
    public const int HatchGroupDataSectionIdentifierIndex = 6;
    public const int LevelObjectivesDataSectionIdentifierIndex = 8;
    public const int PrePlacedLemmingDataSectionIdentifierIndex = 10;
    public const int TerrainDataSectionIdentifierIndex = 12;
    public const int TerrainGroupDataSectionIdentifierIndex = 14;
    public const int GadgetDataSectionIdentifierIndex = 16;

    public static ReadOnlySpan<byte> LevelDataSectionIdentifierBytes =>
    [
        0x26, 0x44,
        0x79, 0xA6,
        0x43, 0xAA,
        0x90, 0xD2,
        0xBE, 0xF4,
        0xFE, 0x77,
        0x60, 0xBB,
        0x7C, 0x5C,
        0x3D, 0x98
    ];

    #region Level Data Read/Write Bits

    public const int UnspecifiedLevelStartValue = 5000;

    #endregion

    #region Level Text Data Read/Write Bits

    #endregion

    #region Hatch Group Data Read/Write Bits

    #endregion

    #region Level Objectives Data Read/Write Bits

    public const int NumberOfBytesForMainLevelObjectiveData = 7;
    public const int NumberOfBytesPerSkillSetDatum = 3;
    public const int NumberOfBytesPerRequirementsDatum = 4;

    public const byte SaveRequirementId = 0x01;
    public const byte TimeRequirementId = 0x02;
    public const byte BasicSkillSetRequirementId = 0x03;

    #endregion

    #region Pre-placed Lemming Data Read/Write Bits

    #endregion

    #region Terrain Data Read/Write Bits

    private const int TerrainDataEraseBitShift = 0;
    private const int TerrainDataNoOverwriteBitShift = 1;
    private const int TerrainDataTintBitShift = 2;
    private const int TerrainDataResizeWidthBitShift = 3;
    private const int TerrainDataResizeHeightBitShift = 4;

    public const int NumberOfBytesForMainTerrainData = 9;

    public static byte EncodeTerrainArchetypeDataByte(TerrainData terrainData)
    {
        var miscDataBits = (terrainData.Erase ? 1 << TerrainDataEraseBitShift : 0) |
                           (terrainData.NoOverwrite ? 1 << TerrainDataNoOverwriteBitShift : 0) |
                           (terrainData.Tint.HasValue ? 1 << TerrainDataTintBitShift : 0) |
                           (terrainData.Width.HasValue ? 1 << TerrainDataResizeWidthBitShift : 0) |
                           (terrainData.Height.HasValue ? 1 << TerrainDataResizeHeightBitShift : 0);

        return (byte)miscDataBits;
    }

    public static DecodedTerrainDataMisc DecodeTerrainDataMiscByte(int terrainDataMiscByte)
    {
        var erase = ((terrainDataMiscByte >>> TerrainDataEraseBitShift) & 1) != 0;
        var noOverwrite = ((terrainDataMiscByte >>> TerrainDataNoOverwriteBitShift) & 1) != 0;
        var hasTintSpecified = ((terrainDataMiscByte >>> TerrainDataTintBitShift) & 1) != 0;
        var hasWidthSpecified = ((terrainDataMiscByte >>> TerrainDataResizeWidthBitShift) & 1) != 0;
        var hasHeightSpecified = ((terrainDataMiscByte >>> TerrainDataResizeHeightBitShift) & 1) != 0;

        return new DecodedTerrainDataMisc(
            erase,
            noOverwrite,
            hasTintSpecified,
            hasWidthSpecified,
            hasHeightSpecified);
    }

    public readonly ref struct DecodedTerrainDataMisc(bool erase, bool noOverwrite, bool hasTintSpecified, bool hasWidthSpecified, bool hasHeightSpecified)
    {
        public readonly bool Erase = erase;
        public readonly bool NoOverwrite = noOverwrite;
        public readonly bool HasTintSpecified = hasTintSpecified;
        public readonly bool HasWidthSpecified = hasWidthSpecified;
        public readonly bool HasHeightSpecified = hasHeightSpecified;
    }

    #endregion

    #region Terrain Group Data Read/Write Bits

    #endregion

    #region Gadget Data Read/Write Bits

    #endregion

    public static uint EncodeTerrainArchetypeDataByte(
        bool isSteel,
        ResizeType resizeType)
    {
        var result = (uint)resizeType;
        result &= 3U;
        var steelValue = isSteel ? 1U : 0U;
        result |= steelValue << 2;
        return result;
    }

    public static void DecodeTerrainArchetypeDataByte(
        uint byteValue,
        out bool isSteel,
        out ResizeType resizeType)
    {
        isSteel = ((byteValue >>> 2) & 1U) != 0U;
        resizeType = (ResizeType)(byteValue & 3U);
    }

    public static void AssertDihedralTransformationByteMakesSense(int dhtByte)
    {
        const int upperBitsMask = ~7;

        if ((dhtByte & upperBitsMask) == 0)
            return;

        throw new LevelReadingException("Read suspicious dihedral transformation byte!");
    }
}
