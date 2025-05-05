using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.LevelIo.Data;
using NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Default.Sections;
using NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Default.Styles;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Default;

public sealed class DefaultLevelReader : ILevelReader
{
    private readonly RawLevelFileDataReader _rawFileData;

    public DefaultLevelReader(string filePath)
    {
        _rawFileData = new RawLevelFileDataReader(filePath);
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

        var version = _rawFileData.Version;
        var stringIdLookup = new List<string>(LevelReadWriteHelpers.InitialStringListCapacity);

        var terrainComponentReader = new TerrainDataSectionReader(version, stringIdLookup);
        ReadOnlySpan<LevelDataSectionReader> sectionReaders =
        [
            // Always process string data first
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

        ReadSections(result, sectionReaders);

        return result;
    }

    private void ReadSections(LevelData result, ReadOnlySpan<LevelDataSectionReader> sectionReaders)
    {
        foreach (var sectionReader in sectionReaders)
        {
            var sectionIdentifier = sectionReader.SectionIdentifier;

            if (!_rawFileData.TryGetSectionInterval(sectionIdentifier, out var interval))
            {
                FileReadingException.ReaderAssert(
                    !sectionReader.IsNecessary,
                    "No data for necessary section!");
                continue;
            }

            _rawFileData.SetReaderPosition(interval.Start);

            var sectionIdentifierBytes = _rawFileData.ReadBytes(LevelReadWriteHelpers.NumberOfBytesForLevelSectionIdentifier);

            FileReadingException.ReaderAssert(
                sectionIdentifierBytes.SequenceEqual(sectionReader.GetSectionIdentifierBytes()),
                "Section Identifier mismatch!");

            sectionReader.ReadSection(_rawFileData, result);

            FileReadingException.ReaderAssert(
                interval.Start + interval.Length == _rawFileData.Position,
                "Byte reading mismatch!");
        }
    }

    public void Dispose()
    {
    }
}