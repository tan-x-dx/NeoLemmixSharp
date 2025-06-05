using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Terrain;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Writing.Levels.Sections.Version1_0_0_0;

internal sealed class TerrainDataSectionWriter : LevelDataSectionWriter
{
    private readonly StringIdLookup _stringIdLookup;

    public TerrainDataSectionWriter(StringIdLookup stringIdLookup)
        : base(LevelFileSectionIdentifier.TerrainDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.AllTerrainData.Count;
    }

    public override void WriteSection(
        RawLevelFileDataWriter writer,
        LevelData levelData)
    {
        foreach (var terrainData in levelData.AllTerrainData)
        {
            WriteTerrainData(writer, terrainData);
        }
    }

    public void WriteTerrainData(
        RawLevelFileDataWriter writer,
        TerrainData terrainData)
    {
        writer.Write(_stringIdLookup.GetStringId(terrainData.StyleIdentifier));
        writer.Write(_stringIdLookup.GetStringId(terrainData.PieceIdentifier));

        writer.Write((ushort)(terrainData.Position.X + ReadWriteHelpers.PositionOffset));
        writer.Write((ushort)(terrainData.Position.Y + ReadWriteHelpers.PositionOffset));
        writer.Write((byte)DihedralTransformation.Encode(terrainData.Orientation, terrainData.FacingDirection));

        WriteTerrainDataMisc(writer, terrainData);
    }

    private static void WriteTerrainDataMisc(
        RawLevelFileDataWriter writer,
        TerrainData terrainData)
    {
        const byte ZeroByte = 0;

        var miscDataBits = ReadWriteHelpers.EncodeTerrainArchetypeDataByte(terrainData);

        writer.Write(miscDataBits);

        if (terrainData.Tint.HasValue)
        {
            var tint = terrainData.Tint.Value;
            writer.Write(tint.R);
            writer.Write(tint.G);
            writer.Write(tint.B);
        }
        else
        {
            writer.Write(ZeroByte);
            writer.Write(ZeroByte);
            writer.Write(ZeroByte);
        }

        if (terrainData.Width.HasValue)
        {
            writer.Write((ushort)terrainData.Width.Value);
        }
        else
        {
            writer.Write(ZeroByte);
            writer.Write(ZeroByte);
        }

        if (terrainData.Height.HasValue)
        {
            writer.Write((ushort)terrainData.Height.Value);
        }
        else
        {
            writer.Write(ZeroByte);
            writer.Write(ZeroByte);
        }
    }
}
