using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Terrain;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Util;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

internal sealed class TerrainDataSectionReader : LevelDataSectionReader
{
    private readonly FileReaderStringIdLookup _stringIdLookup;

    public TerrainDataSectionReader(
        FileReaderStringIdLookup stringIdLookup)
        : base(LevelFileSectionIdentifier.TerrainDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileDataReader reader, LevelData levelData, int numberOfItemsInSection)
    {
        levelData.AllTerrainInstanceData.Capacity = numberOfItemsInSection;

        while (numberOfItemsInSection-- > 0)
        {
            var newTerrainDatum = ReadNextTerrainData(reader);
            levelData.AllTerrainInstanceData.Add(newTerrainDatum);
        }
    }

    private TerrainInstanceData ReadNextTerrainData(RawLevelFileDataReader reader)
    {
        int styleId = reader.Read16BitUnsignedInteger();
        int pieceId = reader.Read16BitUnsignedInteger();

        int positionData = reader.Read32BitSignedInteger();
        var position = ReadWriteHelpers.DecodePoint(positionData);

        int dhtByte = reader.Read8BitUnsignedInteger();
        ReadWriteHelpers.AssertDihedralTransformationByteMakesSense(dhtByte);
        var dht = new DihedralTransformation(dhtByte);

        uint hueAngle = ReadTerrainDataHueAngle(reader);

        uint terrainDataMiscByte = reader.Read8BitUnsignedInteger();
        var decipheredTerrainDataMisc = ReadWriteHelpers.DecodeTerrainDataMiscByte(terrainDataMiscByte);

        int? width = reader.Read16BitUnsignedInteger();
        if (!decipheredTerrainDataMisc.HasWidthSpecified)
        {
            width = null;
        }

        int? height = reader.Read16BitUnsignedInteger();
        if (!decipheredTerrainDataMisc.HasHeightSpecified)
        {
            height = null;
        }

        return new TerrainInstanceData
        {
            GroupName = null,
            StyleIdentifier = new StyleIdentifier(_stringIdLookup[styleId]),
            PieceIdentifier = new PieceIdentifier(_stringIdLookup[pieceId]),

            Position = position,

            NoOverwrite = decipheredTerrainDataMisc.NoOverwrite,
            Orientation = dht.Orientation,
            FacingDirection = dht.FacingDirection,
            Erase = decipheredTerrainDataMisc.Erase,

            HueAngle = hueAngle,

            Width = width,
            Height = height,
        };
    }

    private static uint ReadTerrainDataHueAngle(RawLevelFileDataReader reader)
    {
        uint bytes = reader.Read16BitUnsignedInteger();

        return bytes % 360;
    }
}
