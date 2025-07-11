using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Reading.Levels.Sections;
using NeoLemmixSharp.IO.Versions;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Reading.Levels;

internal readonly ref struct DefaultLevelReader : ILevelReader<DefaultLevelReader>
{
    private readonly RawLevelFileDataReader _reader;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DefaultLevelReader Create(string filePath) => new(filePath);

    private DefaultLevelReader(string filePath)
    {
        using var fileStream = new FileStream(filePath, FileMode.Open);
        _reader = new RawLevelFileDataReader(fileStream);
    }

    public LevelData ReadLevel()
    {
        var levelData = ReadFile();
        levelData.MaxNumberOfClonedLemmings = LevelReadingHelpers.CalculateMaxNumberOfClonedLemmings(levelData);

        return levelData;
    }

    private LevelData ReadFile()
    {
        var result = new LevelData(FileFormatType.Default);

        var fileFormatVersion = _reader.FileFormatVersion;

        var sectionReaders = VersionHelper.GetLevelDataSectionReadersForVersion(fileFormatVersion);

        foreach (var sectionReader in sectionReaders)
        {
            ReadSection(result, sectionReader);
        }

        return result;
    }

    private void ReadSection(LevelData result, LevelDataSectionReader sectionReader)
    {
        if (!_reader.TryGetSectionInterval(sectionReader.SectionIdentifier, out var interval))
        {
            FileReadingException.ReaderAssert(
                !sectionReader.IsNecessary,
                "No data for necessary section!");
            return;
        }

        _reader.SetReaderPosition(interval.Start);

        ushort sectionIdentifierBytes = _reader.Read16BitUnsignedInteger();

        FileReadingException.ReaderAssert(
            sectionIdentifierBytes == sectionReader.GetSectionIdentifier(),
            "Section Identifier mismatch!");

        int numberOfItemsInSection = _reader.Read16BitUnsignedInteger();

        sectionReader.ReadSection(_reader, result, numberOfItemsInSection);

        FileReadingException.ReaderAssert(
            interval.Start + interval.Length == _reader.Position,
            "Byte reading mismatch!");
    }

    public void Dispose()
    {
        _reader?.Dispose();
    }
}
