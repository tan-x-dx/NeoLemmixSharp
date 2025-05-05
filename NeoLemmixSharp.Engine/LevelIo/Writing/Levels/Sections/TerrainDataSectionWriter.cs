using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.LevelIo.Data;
using NeoLemmixSharp.Engine.LevelIo.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelIo.Writing.Levels.Sections;

public sealed class TerrainDataSectionWriter : LevelDataSectionWriter
{
    public override LevelFileSectionIdentifier SectionIdentifier => LevelFileSectionIdentifier.TerrainDataSection;
    public override bool IsNecessary => false;

    private readonly Dictionary<string, ushort> _stringIdLookup;

    public TerrainDataSectionWriter(Dictionary<string, ushort> stringIdLookup)
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
            var terrainArchetypeData = levelData.TerrainArchetypeData[terrainData.GetStylePiecePair()];

            WriteTerrainData(writer, terrainArchetypeData, terrainData);
        }
    }

    public void WriteTerrainData(
        RawLevelFileDataWriter writer,
        TerrainArchetypeData terrainArchetypeData,
        TerrainData terrainData)
    {
        writer.Write(GetNumberOfBytesWritten(terrainData));

        writer.Write(_stringIdLookup[terrainArchetypeData.Style]);
        writer.Write(_stringIdLookup[terrainArchetypeData.TerrainPiece]);

        writer.Write((ushort)(terrainData.Position.X + LevelReadWriteHelpers.PositionOffset));
        writer.Write((ushort)(terrainData.Position.Y + LevelReadWriteHelpers.PositionOffset));
        writer.Write((byte)DihedralTransformation.Encode(terrainData.Orientation, terrainData.FacingDirection));

        WriteTerrainDataMisc(writer, terrainData);
    }

    private static byte GetNumberOfBytesWritten(
        TerrainData terrainData)
    {
        var tintBytes = terrainData.Tint.HasValue ? 3 : 0;
        var resizeBytes = terrainData.Width.HasValue ||
                          terrainData.Height.HasValue ? 4 : 0;

        return (byte)(LevelReadWriteHelpers.NumberOfBytesForMainTerrainData + tintBytes + resizeBytes);
    }

    private static void WriteTerrainDataMisc(
        RawLevelFileDataWriter writer,
        TerrainData terrainData)
    {
        var miscDataBits = LevelReadWriteHelpers.EncodeTerrainArchetypeDataByte(terrainData);

        writer.Write(miscDataBits);

        if (terrainData.Tint.HasValue)
        {
            var tint = terrainData.Tint.Value;
            writer.Write(tint.R);
            writer.Write(tint.G);
            writer.Write(tint.B);
        }

        if (terrainData.Width.HasValue)
        {
            writer.Write((ushort)terrainData.Width.Value);
        }

        if (terrainData.Height.HasValue)
        {
            writer.Write((ushort)terrainData.Height.Value);
        }
    }
}