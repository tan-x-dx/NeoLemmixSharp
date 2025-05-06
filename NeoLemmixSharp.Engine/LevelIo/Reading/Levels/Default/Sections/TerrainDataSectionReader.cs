using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.LevelIo.Data;
using NeoLemmixSharp.Engine.LevelIo.Data.Terrain;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Default.Sections;

public sealed class TerrainDataSectionReader : LevelDataSectionReader
{
    public override LevelFileSectionIdentifier SectionIdentifier => LevelFileSectionIdentifier.TerrainDataSection;
    public override bool IsNecessary => false;

    private readonly List<string> _stringIdLookup;

    public TerrainDataSectionReader(
        Version version,
        List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData)
    {
        int numberOfItemsInSection = rawFileData.Read16BitUnsignedInteger();
        levelData.AllTerrainData.Capacity = numberOfItemsInSection;

        while (numberOfItemsInSection-- > 0)
        {
            var newTerrainDatum = ReadNextTerrainData(rawFileData);
            levelData.AllTerrainData.Add(newTerrainDatum);
        }
    }

    private TerrainData ReadNextTerrainData(RawLevelFileDataReader rawFileData)
    {
        int numberOfBytesToRead = rawFileData.Read8BitUnsignedInteger();
        int initialPosition = rawFileData.Position;

        int styleId = rawFileData.Read16BitUnsignedInteger();
        int pieceId = rawFileData.Read16BitUnsignedInteger();

        int x = rawFileData.Read16BitUnsignedInteger();
        int y = rawFileData.Read16BitUnsignedInteger();

        x -= LevelReadWriteHelpers.PositionOffset;
        y -= LevelReadWriteHelpers.PositionOffset;

        int dhtByte = rawFileData.Read8BitUnsignedInteger();
        LevelReadWriteHelpers.AssertDihedralTransformationByteMakesSense(dhtByte);
        var dht = new DihedralTransformation(dhtByte);

        int terrainDataMiscByte = rawFileData.Read8BitUnsignedInteger();
        var decipheredTerrainDataMisc = LevelReadWriteHelpers.DecodeTerrainDataMiscByte(terrainDataMiscByte);

        Color? tintColor = null;
        if (decipheredTerrainDataMisc.HasTintSpecified)
        {
            tintColor = ReadTerrainDataTintColor(rawFileData);
        }

        int? width = null;
        if (decipheredTerrainDataMisc.HasWidthSpecified)
        {
            width = rawFileData.Read16BitUnsignedInteger();
        }

        int? height = null;
        if (decipheredTerrainDataMisc.HasHeightSpecified)
        {
            height = rawFileData.Read16BitUnsignedInteger();
        }

        FileReadingException.AssertBytesMakeSense(
            rawFileData.Position,
            initialPosition,
            numberOfBytesToRead,
            "terrain data");

        return new TerrainData
        {
            GroupName = null,
            Style = _stringIdLookup[styleId],
            TerrainPiece = _stringIdLookup[pieceId],

            Position = new Point(x, y),

            NoOverwrite = decipheredTerrainDataMisc.NoOverwrite,
            Orientation = dht.Orientation,
            FacingDirection = dht.FacingDirection,
            Erase = decipheredTerrainDataMisc.Erase,

            Tint = tintColor,

            Width = width,
            Height = height,
        };
    }

    private static Color ReadTerrainDataTintColor(RawLevelFileDataReader rawFileData)
    {
        return rawFileData.ReadRgbColor();
    }
}