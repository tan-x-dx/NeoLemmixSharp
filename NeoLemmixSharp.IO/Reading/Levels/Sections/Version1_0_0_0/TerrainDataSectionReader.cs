using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Terrain;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Util;
using Color = Microsoft.Xna.Framework.Color;

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

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData, int numberOfItemsInSection)
    {
        levelData.AllTerrainData.Capacity = numberOfItemsInSection;

        while (numberOfItemsInSection-- > 0)
        {
            var newTerrainDatum = ReadNextTerrainData(rawFileData);
            levelData.AllTerrainData.Add(newTerrainDatum);
        }
    }

    private TerrainData ReadNextTerrainData(RawLevelFileDataReader rawFileData)
    {
        int styleId = rawFileData.Read16BitUnsignedInteger();
        int pieceId = rawFileData.Read16BitUnsignedInteger();

        int positionData = rawFileData.Read32BitSignedInteger();
        var position = ReadWriteHelpers.DecodePoint(positionData);

        int dhtByte = rawFileData.Read8BitUnsignedInteger();
        ReadWriteHelpers.AssertDihedralTransformationByteMakesSense(dhtByte);
        var dht = new DihedralTransformation(dhtByte);

        uint terrainDataMiscByte = rawFileData.Read8BitUnsignedInteger();
        var decipheredTerrainDataMisc = ReadWriteHelpers.DecodeTerrainDataMiscByte(terrainDataMiscByte);

        Color? tintColor = ReadTerrainDataTintColor(rawFileData);
        if (!decipheredTerrainDataMisc.HasTintSpecified)
        {
            tintColor = null;
        }

        int? width = rawFileData.Read16BitUnsignedInteger();
        if (!decipheredTerrainDataMisc.HasWidthSpecified)
        {
            width = null;
        }

        int? height = rawFileData.Read16BitUnsignedInteger();
        if (!decipheredTerrainDataMisc.HasHeightSpecified)
        {
            height = null;
        }

        return new TerrainData
        {
            GroupName = null,
            StyleIdentifier = new StyleIdentifier(_stringIdLookup[styleId]),
            PieceIdentifier = new PieceIdentifier(_stringIdLookup[pieceId]),

            Position = position,

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

        return ReadWriteHelpers.ReadRgbBytes(bytes);
    }
}
