using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.IO.Data.Level.Terrain;
using NeoLemmixSharp.IO.Reading;
using NeoLemmixSharp.IO.Writing;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO;

public static class LevelReadWriteHelpers
{
    public const int PositionOffset = 512;

    public const int InitialStringListCapacity = 32;

    public const long MaxAllowedFileSizeInBytes = 1024 * 1024 * 64;
    public const string FileSizeTooLargeExceptionMessage = "File too large! Max file size is 64Mb";

    public sealed class SectionIdentifierComparer<TPerfectHasher, TEnum> : IComparer<Interval>
        where TPerfectHasher : struct, ISectionIdentifierHelper<TEnum>
        where TEnum : unmanaged, Enum
    {
        [SkipLocalsInit]
        public void AssertSectionsAreContiguous(BitArrayDictionary<TPerfectHasher, BitBuffer32, TEnum, Interval> result)
        {
            Span<Interval> intervals = stackalloc Interval[result.Count];
            result.CopyValuesTo(intervals);

            intervals.Sort(this);

            for (var i = 0; i < intervals.Length - 1; i++)
            {
                var firstInterval = intervals[i];
                var secondInterval = intervals[i + 1];

                if (firstInterval.Start + firstInterval.Length != secondInterval.Start)
                    throw new InvalidOperationException("Sections are not contiguous!");
            }
        }

        int IComparer<Interval>.Compare(Interval x, Interval y)
        {
            return x.Start.CompareTo(y.Start);
        }
    }

    #region Level Data Read/Write Stuff

    public const int UnspecifiedLevelStartValue = 5000;

    #endregion

    #region Level Text Data Read/Write Stuff

    #endregion

    #region Hatch Group Data Read/Write Stuff

    #endregion

    #region Level Objectives Data Read/Write Stuff

    public const int NumberOfBytesForMainLevelObjectiveData = 7;
    public const int NumberOfBytesPerSkillSetDatum = 3;
    public const int NumberOfBytesPerRequirementsDatum = 4;

    public const byte SaveRequirementId = 0x01;
    public const byte TimeRequirementId = 0x02;
    public const byte BasicSkillSetRequirementId = 0x03;

    #endregion

    #region Pre-placed Lemming Data Read/Write Stuff

    #endregion

    #region Terrain Data Read/Write Stuff

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

    #region Terrain Group Data Read/Write Stuff

    #endregion

    #region Gadget Data Read/Write Stuff

    #endregion

    #region Terrain Archetype Data Read/Write Stuff

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

    #endregion

    #region Gadget Archetype Data Read/Write Stuff

    private const int GadgetArchetypeDataAllowedActionsBitShift = 0;
    private const int GadgetArchetypeDataAllowedStatesBitShift = 1;
    private const int GadgetArchetypeDataAllowedOrientationsBitShift = 2;
    private const int GadgetArchetypeDataAllowedFacingDirectionBitShift = 3;

    public static uint EncodeGadgetArchetypeDataFilterByte(
        DecodedGadgetArchetypeDataHitBoxFilter hitBoxFilter)
    {
        var filterByte = (hitBoxFilter.HasAllowedActions ? 1 << GadgetArchetypeDataAllowedActionsBitShift : 0) |
                         (hitBoxFilter.HasAllowedStates ? 1 << GadgetArchetypeDataAllowedStatesBitShift : 0) |
                         (hitBoxFilter.HasAllowedOrientations ? 1 << GadgetArchetypeDataAllowedOrientationsBitShift : 0) |
                         (hitBoxFilter.HasAllowedFacingDirection ? 1 << GadgetArchetypeDataAllowedFacingDirectionBitShift : 0);

        return (byte)filterByte;
    }

    public static DecodedGadgetArchetypeDataHitBoxFilter DecodeGadgetArchetypeDataFilterByte(
        uint byteValue)
    {
        var hasAllowedActions = ((byteValue >>> GadgetArchetypeDataAllowedActionsBitShift) & 1) != 0;
        var hasAllowedStates = ((byteValue >>> GadgetArchetypeDataAllowedStatesBitShift) & 1) != 0;
        var hasAllowedOrientations = ((byteValue >>> GadgetArchetypeDataAllowedOrientationsBitShift) & 1) != 0;
        var hasAllowedFacingDirection = ((byteValue >>> GadgetArchetypeDataAllowedFacingDirectionBitShift) & 1) != 0;

        return new DecodedGadgetArchetypeDataHitBoxFilter(hasAllowedActions, hasAllowedStates, hasAllowedOrientations, hasAllowedFacingDirection);
    }

    public readonly ref struct DecodedGadgetArchetypeDataHitBoxFilter(bool hasAllowedActions, bool hasAllowedStates, bool hasAllowedOrientations, bool hasAllowedFacingDirection)
    {
        public readonly bool HasAllowedActions = hasAllowedActions;
        public readonly bool HasAllowedStates = hasAllowedStates;
        public readonly bool HasAllowedOrientations = hasAllowedOrientations;
        public readonly bool HasAllowedFacingDirection = hasAllowedFacingDirection;
    }

    #endregion

    public static void AssertDihedralTransformationByteMakesSense(int dhtByte)
    {
        const int upperBitsMask = ~7;

        FileReadingException.ReaderAssert((dhtByte & upperBitsMask) == 0, "Read suspicious dihedral transformation byte!");
    }

    public static void WriteArgbBytes(Color color, Span<byte> bytes)
    {
        Debug.Assert(bytes.Length == 4);
        bytes[0] = color.A;
        bytes[1] = color.R;
        bytes[2] = color.G;
        bytes[3] = color.B;
    }

    public static void WriteRgbBytes(Color color, Span<byte> bytes)
    {
        Debug.Assert(bytes.Length == 3);
        bytes[0] = color.R;
        bytes[1] = color.G;
        bytes[2] = color.B;
    }

    public static Color ReadArgbBytes(ReadOnlySpan<byte> bytes)
    {
        Debug.Assert(bytes.Length == 4);
        return new Color(alpha: bytes[0], r: bytes[1], g: bytes[2], b: bytes[3]);
    }

    public static Color ReadRgbBytes(ReadOnlySpan<byte> bytes)
    {
        const byte alphaByte = 0xff;
        Debug.Assert(bytes.Length == 3);
        return new Color(alpha: alphaByte, r: bytes[0], g: bytes[1], b: bytes[2]);
    }
}
