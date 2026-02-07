using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level.Terrain;
using NeoLemmixSharp.IO.Reading;
using NeoLemmixSharp.IO.Writing;

namespace NeoLemmixSharp.IO.Util;

public static class ReadWriteHelpers
{
    #region Level Data Read/Write Stuff

    internal const int UnspecifiedLevelStartValue = 5000;

    #endregion

    #region Terrain Data Read/Write Stuff

    private const int TerrainDataEraseBitShift = 0;
    private const int TerrainDataNoOverwriteBitShift = 1;

    internal static byte EncodeTerrainArchetypeDataByte(TerrainInstanceData terrainData)
    {
        var miscDataBits = (terrainData.Erase ? 1 << TerrainDataEraseBitShift : 0) |
                           (terrainData.NoOverwrite ? 1 << TerrainDataNoOverwriteBitShift : 0);

        return (byte)miscDataBits;
    }

    internal static DecodedTerrainDataMisc DecodeTerrainDataMiscByte(uint terrainDataMiscByte)
    {
        var erase = ((terrainDataMiscByte >>> TerrainDataEraseBitShift) & 1) != 0;
        var noOverwrite = ((terrainDataMiscByte >>> TerrainDataNoOverwriteBitShift) & 1) != 0;

        return new DecodedTerrainDataMisc(
            erase,
            noOverwrite);
    }

    internal readonly ref struct DecodedTerrainDataMisc(bool erase, bool noOverwrite)
    {
        internal readonly bool Erase = erase;
        internal readonly bool NoOverwrite = noOverwrite;
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

    #region NeoLemmix Compat Helpers

    public static Texture2D GetOrLoadTerrainTexture(StyleIdentifier styleIdentifier, PieceIdentifier pieceIdentifier, string terrainPieceFilePath)
    {
        // Need to load the texture here to get its dimensions, since that data is not present in the NeoLemmix config file
        var pngFilePath = RootDirectoryManager.GetCorrespondingImageFile(terrainPieceFilePath);
        return TextureCache.GetOrLoadTexture(pngFilePath, styleIdentifier, pieceIdentifier, TextureType.TerrainSprite);
    }

    #endregion
}
