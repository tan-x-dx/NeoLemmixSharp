using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Reading;
using NeoLemmixSharp.IO.Writing;

namespace NeoLemmixSharp.IO.Util;

public static class ReadWriteHelpers
{
    #region Level Data Read/Write Stuff

    internal const int UnspecifiedLevelStartValue = 5000;

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

    internal static PackedBytes32 EncodePoint(Point point)
    {
        FileWritingException.WriterAssert(point.X <= short.MaxValue && point.X >= short.MinValue, "Point outside of valid scope");
        FileWritingException.WriterAssert(point.Y <= short.MaxValue && point.Y >= short.MinValue, "Point outside of valid scope");

        var result = new PackedBytes32();
        result.SetUshort(0, point.X);
        result.SetUshort(1, point.Y);

        return result;
    }

    public static Point DecodePoint(PackedBytes32 packedBytes)
    {
        int x = (short)packedBytes.GetUshort(0);
        int y = (short)packedBytes.GetUshort(1);

        return new Point(x, y);
    }
}
