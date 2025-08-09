﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level.Terrain;
using NeoLemmixSharp.IO.Reading;
using NeoLemmixSharp.IO.Writing;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.IO.Util;

public static class ReadWriteHelpers
{
    #region Level Data Read/Write Stuff

    internal const int UnspecifiedLevelStartValue = 5000;

    #endregion

    #region Level Text Data Read/Write Stuff

    #endregion

    #region Hatch Group Data Read/Write Stuff

    #endregion

    #region Level Objectives Data Read/Write Stuff

    #endregion

    #region Pre-placed Lemming Data Read/Write Stuff

    #endregion

    #region Terrain Data Read/Write Stuff

    private const int TerrainDataEraseBitShift = 0;
    private const int TerrainDataNoOverwriteBitShift = 1;
    private const int TerrainDataTintBitShift = 2;
    private const int TerrainDataResizeWidthBitShift = 3;
    private const int TerrainDataResizeHeightBitShift = 4;

    internal const int NumberOfBytesForMainTerrainData = 9;

    internal static byte EncodeTerrainArchetypeDataByte(TerrainData terrainData)
    {
        var miscDataBits = (terrainData.Erase ? 1 << TerrainDataEraseBitShift : 0) |
                           (terrainData.NoOverwrite ? 1 << TerrainDataNoOverwriteBitShift : 0) |
                           (terrainData.Tint.HasValue ? 1 << TerrainDataTintBitShift : 0) |
                           (terrainData.Width.HasValue ? 1 << TerrainDataResizeWidthBitShift : 0) |
                           (terrainData.Height.HasValue ? 1 << TerrainDataResizeHeightBitShift : 0);

        return (byte)miscDataBits;
    }

    internal static DecodedTerrainDataMisc DecodeTerrainDataMiscByte(uint terrainDataMiscByte)
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

    internal readonly ref struct DecodedTerrainDataMisc(bool erase, bool noOverwrite, bool hasTintSpecified, bool hasWidthSpecified, bool hasHeightSpecified)
    {
        internal readonly bool Erase = erase;
        internal readonly bool NoOverwrite = noOverwrite;
        internal readonly bool HasTintSpecified = hasTintSpecified;
        internal readonly bool HasWidthSpecified = hasWidthSpecified;
        internal readonly bool HasHeightSpecified = hasHeightSpecified;
    }

    #endregion

    #region Terrain Group Data Read/Write Stuff

    #endregion

    #region Gadget Data Read/Write Stuff

    #endregion

    #region Terrain Archetype Data Read/Write Stuff

    internal static uint EncodeTerrainArchetypeDataByte(
        bool isSteel,
        ResizeType resizeType)
    {
        var result = (uint)resizeType;
        result &= 3U;
        var steelValue = isSteel ? 1U : 0U;
        result |= steelValue << 2;
        return result;
    }

    internal static void DecodeTerrainArchetypeDataByte(
        uint byteValue,
        out bool isSteel,
        out ResizeType resizeType)
    {
        isSteel = ((byteValue >>> 2) & 1U) != 0U;
        resizeType = DecodeResizeType(byteValue);
    }

    internal static ResizeType DecodeResizeType(uint byteValue)
    {
        return (ResizeType)(byteValue & 3U);
    }

    #endregion

    #region Gadget Archetype Data Read/Write Stuff

    #endregion

    internal static void AssertDihedralTransformationByteMakesSense(int dhtByte)
    {
        const int upperBitsMask = ~7;

        FileReadingException.ReaderAssert((dhtByte & upperBitsMask) == 0, "Read suspicious dihedral transformation byte!");
    }

    internal static void WriteArgbBytes(Color color, Span<byte> bytes)
    {
        FileWritingException.WriterAssert(bytes.Length == 4, "Expected span length of exactly 4");
        bytes[0] = color.A;
        bytes[1] = color.R;
        bytes[2] = color.G;
        bytes[3] = color.B;
    }

    internal static void WriteRgbBytes(Color color, Span<byte> bytes)
    {
        FileWritingException.WriterAssert(bytes.Length == 3, "Expected span length of exactly 3");
        bytes[0] = color.R;
        bytes[1] = color.G;
        bytes[2] = color.B;
    }

    internal static Color ReadArgbBytes(ReadOnlySpan<byte> bytes)
    {
        FileReadingException.ReaderAssert(bytes.Length == 4, "Expected span length of exactly 4");
        return new Color(alpha: bytes[0], r: bytes[1], g: bytes[2], b: bytes[3]);
    }

    internal static Color ReadRgbBytes(ReadOnlySpan<byte> bytes)
    {
        const byte alphaByte = 0xff;
        FileReadingException.ReaderAssert(bytes.Length == 3, "Expected span length of exactly 3");
        return new Color(alpha: alphaByte, r: bytes[0], g: bytes[1], b: bytes[2]);
    }

    internal static int EncodePoint(Point point)
    {
        FileWritingException.WriterAssert(point.X <= short.MaxValue && point.X >= short.MinValue, "Point outside of valid scope");
        FileWritingException.WriterAssert(point.Y <= short.MaxValue && point.Y >= short.MinValue, "Point outside of valid scope");

        return (point.Y & 0xffff) << 16 | point.X & 0xffff;
    }

    public static Point DecodePoint(int combinedBits)
    {
        short x = (short)(combinedBits & 0xffff);
        short y = (short)(combinedBits >>> 16);

        return new Point(x, y);
    }
}
