using NeoLemmixSharp.Engine.LevelBuilding.Data;
using System.Runtime.CompilerServices;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Components;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

public sealed class DataReaderList : IDisposable
{
    private readonly ILevelDataReader[] _dataReaders;
    private readonly BinaryReaderWrapper _binaryReaderWrapper;

    public DataReaderList(string filePath)
    {
        _binaryReaderWrapper = new BinaryReaderWrapper(filePath);

        var stringIdLookup = new Dictionary<ushort, string>();

        var terrainComponentReader = new TerrainDataComponentReader(stringIdLookup);
        _dataReaders =
        [
            new StringDataComponentReader(stringIdLookup),

            new LevelDataComponentReader(stringIdLookup),
            new LevelTextDataComponentReader(stringIdLookup),
            new HatchGroupDataComponentReader(),
            new LevelObjectiveDataComponentReader(stringIdLookup),
            new PrePlacedLemmingDataComponentReader(),
            terrainComponentReader,
            new TerrainGroupDataComponentReader(stringIdLookup, terrainComponentReader),
            new GadgetDataComponentReader(stringIdLookup)
        ];
    }

    public LevelData ReadFile()
    {
        var version = ReadVersion();

        var result = new LevelData();

        while (_binaryReaderWrapper.MoreToRead)
        {
            GetNextDataReader().ReadSection(_binaryReaderWrapper, result);
        }

        return result;
    }

    private Version ReadVersion()
    {
        var major = _binaryReaderWrapper.Read16BitUnsignedInteger();
        AssertNextByteIsPeriod();
        var minor = _binaryReaderWrapper.Read16BitUnsignedInteger();
        AssertNextByteIsPeriod();
        var build = _binaryReaderWrapper.Read16BitUnsignedInteger();
        AssertNextByteIsPeriod();
        var revision = _binaryReaderWrapper.Read16BitUnsignedInteger();

        return new Version(major, minor, build, revision);

        void AssertNextByteIsPeriod()
        {
            var nextByte = _binaryReaderWrapper.Read8BitUnsignedInteger();

            LevelReadWriteHelpers.ReaderAssert(nextByte == '.', "Version not in correct format");
        }
    }

    [SkipLocalsInit]
    private ILevelDataReader GetNextDataReader()
    {
        Span<byte> buffer = stackalloc byte[2];
        _binaryReaderWrapper.ReadBytes(buffer);

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
        _binaryReaderWrapper.Dispose();
    }
}