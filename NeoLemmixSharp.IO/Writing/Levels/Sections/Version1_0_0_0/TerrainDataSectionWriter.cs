using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Terrain;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Util;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Writing.Levels.Sections.Version1_0_0_0;

internal sealed class TerrainDataSectionWriter : LevelDataSectionWriter
{
    private readonly FileWriterStringIdLookup _stringIdLookup;

    public TerrainDataSectionWriter(FileWriterStringIdLookup stringIdLookup)
        : base(LevelFileSectionIdentifier.TerrainDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.AllTerrainInstanceData.Count;
    }

    public override void WriteSection(
        RawLevelFileDataWriter writer,
        LevelData levelData)
    {
        foreach (var terrainDatum in levelData.AllTerrainInstanceData)
        {
            WriteTerrainData(writer, terrainDatum);
        }
    }

    public void WriteTerrainData(
        RawLevelFileDataWriter writer,
        TerrainInstanceData terrainDatum)
    {
        writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(terrainDatum.StyleIdentifier));
        writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(terrainDatum.PieceIdentifier));

        writer.Write32BitSignedInteger(ReadWriteHelpers.EncodePoint(terrainDatum.Position));
        writer.Write8BitUnsignedInteger((byte)DihedralTransformation.Encode(terrainDatum.Orientation, terrainDatum.FacingDirection));

        WriteTerrainDataMisc(writer, terrainDatum);
    }

    [SkipLocalsInit]
    private static void WriteTerrainDataMisc(
        RawLevelFileDataWriter writer,
        TerrainInstanceData terrainData)
    {
        Span<byte> colorBuffer = stackalloc byte[3];
        ReadWriteHelpers.WriteRgbBytes(terrainData.Tint, colorBuffer);

        writer.WriteBytes(colorBuffer);
        writer.Write16BitUnsignedInteger((ushort)(terrainData.HueAngle % 360));

        var miscDataBits = ReadWriteHelpers.EncodeTerrainArchetypeDataByte(terrainData);

        writer.Write8BitUnsignedInteger(miscDataBits);

        writer.Write16BitUnsignedInteger((ushort)terrainData.Width);
        writer.Write16BitUnsignedInteger((ushort)terrainData.Height);
    }
}
