using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public static class LevelReadWriteHelpers
{
    public const int PositionOffset = 512;

    #region String Data Read/Write Bits

    public static ReadOnlySpan<byte> StringDataSectionIdentifier => [0x26, 0x44];

    #endregion

    #region Level Data Read/Write Bits

    public static ReadOnlySpan<byte> LevelDataSectionIdentifier => [0x79, 0xA6];

    public const uint NoBackgroundSpecified = 0x00;
    public const uint SolidColorBackground = 0x01;
    public const uint TextureBackground = 0x02;

    public const int UnspecifiedLevelStartValue = 5000;

    #endregion

    #region Level Text Data Read/Write Bits

    public static ReadOnlySpan<byte> LevelTextDataSectionIdentifier => [0x43, 0xAA];

    #endregion

    #region Hatch Group Data Read/Write Bits

    public static ReadOnlySpan<byte> HatchGroupDataSectionIdentifier => [0x90, 0xD2];

    #endregion

    #region Level Objectives Data Read/Write Bits

    public static ReadOnlySpan<byte> LevelObjectivesDataSectionIdentifier => [0xBE, 0xF4];

    public const int NumberOfBytesForMainLevelObjectiveData = 7;
    public const int NumberOfBytesPerSkillSetDatum = 3;
    public const int NumberOfBytesPerRequirementsDatum = 4;

    public const byte SaveRequirementId = 0x01;
    public const byte TimeRequirementId = 0x02;
    public const byte BasicSkillSetRequirementId = 0x03;

    #endregion

    #region Pre-placed Lemming Data Read/Write Bits

    public static ReadOnlySpan<byte> PrePlacedLemmingDataSectionIdentifier => [0xFE, 0x77];

    #endregion

    #region Terrain Data Read/Write Bits

    public static ReadOnlySpan<byte> TerrainDataSectionIdentifier => [0x60, 0xBB];

    public const int TerrainDataEraseBitShift = 0;
    public const int TerrainDataNoOverwriteBitShift = 1;
    public const int TerrainDataTintBitShift = 2;
    public const int TerrainDataResizeWidthBitShift = 3;
    public const int TerrainDataResizeHeightBitShift = 4;

    public const int NumberOfBytesForMainTerrainData = 9;

    public static DecipheredTerrainDataMisc DecipherTerrainDataMiscByte(byte b)
    {
        int intValue = b;
        var erase = ((intValue >> TerrainDataEraseBitShift) & 1) != 0;
        var noOverwrite = ((intValue >> TerrainDataNoOverwriteBitShift) & 1) != 0;
        var hasTintSpecified = ((intValue >> TerrainDataTintBitShift) & 1) != 0;
        var hasWidthSpecified = ((intValue >> TerrainDataResizeWidthBitShift) & 1) != 0;
        var hasHeightSpecified = ((intValue >> TerrainDataResizeHeightBitShift) & 1) != 0;

        return new DecipheredTerrainDataMisc(
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

    #endregion

    #region Terrain Group Data Read/Write Bits

    public static ReadOnlySpan<byte> TerrainGroupDataSectionIdentifier => [0x7C, 0x5C];

    #endregion

    #region Gadget Data Read/Write Bits

    public static ReadOnlySpan<byte> GadgetDataSectionIdentifier => [0x3D, 0x98];

    #endregion

    public static void ReaderAssert([DoesNotReturnIf(false)] bool condition, string details)
    {
        if (condition)
            return;

        throw new LevelReadingException($"Error occurred when reading level file. Details: [{details}]");
    }

    public static uint GetTerrainArchetypeDataByte(
        bool isSteel,
        ResizeType resizeType)
    {
        var result = (uint)resizeType;
        result &= 3U;
        var steelValue = isSteel ? 1U : 0U;
        result |= steelValue << 2;
        return result;
    }

    public static void DecipherTerrainArchetypeDataByte(
        uint byteValue,
        out bool isSteel,
        out ResizeType resizeType)
    {
        isSteel = ((byteValue >> 2) & 1U) != 0U;
        resizeType = (ResizeType)(byteValue & 3U);
    }
}