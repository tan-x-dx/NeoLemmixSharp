using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.LevelIo.Data.Level;
using NeoLemmixSharp.Engine.LevelIo.Data.Level.Terrain;
using NeoLemmixSharp.Engine.LevelIo.Data.Style;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Sections.Version1_0_0_0;

public sealed class TerrainDataSectionReader : LevelDataSectionReader
{

    private readonly List<string> _stringIdLookup;

    public TerrainDataSectionReader(
        List<string> stringIdLookup)
        : base(LevelFileSectionIdentifier.TerrainDataSection, false)
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
            StyleIdentifier = new StyleIdentifier(_stringIdLookup[styleId]),
            TerrainPiece = new PieceIdentifier(_stringIdLookup[pieceId]),

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
        var bytes = rawFileData.ReadBytes(3);

        return LevelReadWriteHelpers.ReadRgbBytes(bytes);
    }
}