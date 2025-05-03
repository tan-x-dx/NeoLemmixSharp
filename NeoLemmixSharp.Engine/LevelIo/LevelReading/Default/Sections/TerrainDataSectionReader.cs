using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.LevelIo.Data;
using NeoLemmixSharp.Engine.LevelIo.Data.Terrain;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default.Sections;

public sealed class TerrainDataSectionReader : LevelDataSectionReader
{
    private readonly List<string> _stringIdLookup;

    public TerrainDataSectionReader(
        Version version,
        List<string> stringIdLookup)
        : base(LevelFileSectionIdentifier.TerrainDataSection)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileData rawFileData, LevelData levelData)
    {
        int numberOfItemsInSection = rawFileData.Read16BitUnsignedInteger();
        levelData.AllTerrainData.Capacity = numberOfItemsInSection;

        while (numberOfItemsInSection-- > 0)
        {
            var newTerrainDatum = ReadNextTerrainData(rawFileData);
            levelData.AllTerrainData.Add(newTerrainDatum);
        }
    }

    private TerrainData ReadNextTerrainData(RawLevelFileData rawFileData)
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

        AssertTerrainDataBytesMakeSense(
            rawFileData.Position,
            initialPosition,
            numberOfBytesToRead);

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

    private static Color ReadTerrainDataTintColor(RawLevelFileData rawFileData)
    {
        return rawFileData.ReadRgbColor();
    }

    private static void AssertTerrainDataBytesMakeSense(
        int currentPosition,
        int initialPosition,
        int expectedByteCount)
    {
        if (currentPosition - initialPosition == expectedByteCount)
            return;

        throw new LevelReadingException(
            "Wrong number of bytes read for terrain data! " +
            $"Expected: {expectedByteCount}, Actual: {currentPosition - initialPosition}");
    }
}