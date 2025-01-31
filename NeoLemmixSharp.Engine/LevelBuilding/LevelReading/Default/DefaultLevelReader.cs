using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Components;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

public sealed class DefaultLevelReader : ILevelReader
{
    private readonly ILevelDataReader[] _dataReaders;
    private readonly RawFileData _rawFileData;

    public DefaultLevelReader(string filePath)
    {
        _rawFileData = new RawFileData(filePath);

        var version = ReadVersion();

        var stringIdLookup = new List<string>();

        var terrainComponentReader = new TerrainDataComponentReader(version, stringIdLookup);
        _dataReaders =
        [
            new StringDataComponentReader(version, stringIdLookup),

            new LevelDataComponentReader(version, stringIdLookup),
            new LevelTextDataComponentReader(version, stringIdLookup),
            new HatchGroupDataComponentReader(version),
            new LevelObjectiveDataComponentReader(version, stringIdLookup),
            new PrePlacedLemmingDataComponentReader(version),
            terrainComponentReader,
            new TerrainGroupDataComponentReader(version, stringIdLookup, terrainComponentReader),
            new GadgetDataComponentReader(version, stringIdLookup)
        ];
    }

    private Version ReadVersion()
    {
        int major = _rawFileData.Read16BitUnsignedInteger();
        AssertNextByteIsPeriod();
        int minor = _rawFileData.Read16BitUnsignedInteger();
        AssertNextByteIsPeriod();
        int build = _rawFileData.Read16BitUnsignedInteger();
        AssertNextByteIsPeriod();
        int revision = _rawFileData.Read16BitUnsignedInteger();

        return new Version(major, minor, build, revision);

        void AssertNextByteIsPeriod()
        {
            int nextByteValue = _rawFileData.Read8BitUnsignedInteger();

            LevelReadWriteHelpers.ReaderAssert(nextByteValue == '.', "Version not in correct format");
        }
    }

    public LevelData ReadLevel(GraphicsDevice graphicsDevice)
    {
        var levelData = ReadFile();
        levelData.MaxNumberOfClonedLemmings = LevelReadingHelpers.CalculateMaxNumberOfClonedLemmings(levelData);

        return levelData;
    }

    private LevelData ReadFile()
    {
        var result = new LevelData();

        while (_rawFileData.MoreToRead)
        {
            GetNextDataReader().ReadSection(_rawFileData, result);
        }

        return result;
    }

    private ILevelDataReader GetNextDataReader()
    {
        var buffer = _rawFileData.ReadBytes(2);

        foreach (var levelDataWriter in _dataReaders)
        {
            if (buffer.SequenceEqual(levelDataWriter.GetSectionIdentifier()))
            {
                if (levelDataWriter.AlreadyUsed)
                    throw new LevelReadingException(
                        "Attempted to read the same section multiple times!" +
                        $"{levelDataWriter.GetType().Name}");

                return levelDataWriter;
            }
        }

        throw new LevelReadingException($"Unknown section identifier: {buffer[0]:X} {buffer[1]:X}");
    }

    public void Dispose()
    {
    }
}