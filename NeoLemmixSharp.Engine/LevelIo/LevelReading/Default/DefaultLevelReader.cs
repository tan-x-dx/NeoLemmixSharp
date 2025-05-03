using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.LevelIo.Data;
using NeoLemmixSharp.Engine.LevelIo.LevelReading.Default.Sections;
using NeoLemmixSharp.Engine.LevelIo.LevelReading.Default.Styles;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default;

public sealed class DefaultLevelReader : ILevelReader
{
    private readonly RawLevelFileData _rawFileData;

    public DefaultLevelReader(string filePath)
    {
        _rawFileData = new RawLevelFileData(filePath);
    }

    public LevelData ReadLevel(GraphicsDevice graphicsDevice)
    {
        var levelData = ReadFile();
        levelData.MaxNumberOfClonedLemmings = LevelReadingHelpers.CalculateMaxNumberOfClonedLemmings(levelData);

        DefaultStyleHelpers.ProcessStyleArchetypeData(levelData);

        return levelData;
    }

    private LevelData ReadFile()
    {
        var result = new LevelData();

        var sectionReaders = GetSectionReaders(_rawFileData.Version);

        foreach (var sectionReader in sectionReaders)
        {
            var sectionIdentifier = sectionReader.SectionIdentifier;

            if (!_rawFileData.TryGetSectionInterval(sectionIdentifier, out var interval))
                continue;

            _rawFileData.SetReaderPosition(interval.Start);

            var sectionIdentifierBytes = _rawFileData.ReadBytes(LevelReadWriteHelpers.NumberOfBytesForLevelSectionIdentifier);

            LevelReadingException.ReaderAssert(
                sectionIdentifierBytes.SequenceEqual(sectionReader.GetSectionIdentifierBytes()),
                "Section Identifier mismatch!");

            sectionReader.ReadSection(_rawFileData, result);

            LevelReadingException.ReaderAssert(
                interval.Start + interval.Length == _rawFileData.Position,
                "Byte reading mismatch!");
        }

        return result;
    }

    private static LevelDataSectionReader[] GetSectionReaders(Version version)
    {
        var stringIdLookup = new List<string>(16);

        var terrainComponentReader = new TerrainDataSectionReader(version, stringIdLookup);
        return
        [
            new StringDataSectionReader(version, stringIdLookup),

            new LevelMetadataSectionReader(version, stringIdLookup),
            new LevelTextDataSectionReader(version, stringIdLookup),
            new HatchGroupDataSectionReader(version),
            new LevelObjectiveDataSectionReader(version, stringIdLookup),
            new PrePlacedLemmingDataSectionReader(version),
            terrainComponentReader,
            new TerrainGroupDataSectionReader(version, stringIdLookup, terrainComponentReader),
            new GadgetDataSectionReader(version, stringIdLookup)
        ];
    }

    public void Dispose()
    {
    }
}