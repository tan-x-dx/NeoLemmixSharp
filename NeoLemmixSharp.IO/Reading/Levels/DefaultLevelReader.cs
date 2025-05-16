using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Reading.Levels.Sections;
using NeoLemmixSharp.IO.Versions;

namespace NeoLemmixSharp.IO.Reading.Levels;

internal sealed class DefaultLevelReader : ILevelReader
{
    private readonly RawLevelFileDataReader _rawFileData;

    public DefaultLevelReader(string filePath)
    {
        using var fileStream = new FileStream(filePath, FileMode.Open);
        _rawFileData = new RawLevelFileDataReader(fileStream);
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

        var version = _rawFileData.Version;

        var sectionReaders = VersionHelper.GetLevelDataSectionReadersForVersion(version);

        foreach (var sectionReader in sectionReaders)
        {
            ReadSection(result, sectionReader);
        }

        return result;
    }

    private void ReadSection(LevelData result, LevelDataSectionReader sectionReader)
    {
        var sectionIdentifier = sectionReader.SectionIdentifier;

        if (!_rawFileData.TryGetSectionInterval(sectionIdentifier, out var interval))
        {
            FileReadingException.ReaderAssert(
                !sectionReader.IsNecessary,
                "No data for necessary section!");
            return;
        }

        _rawFileData.SetReaderPosition(interval.Start);

        var sectionIdentifierBytes = _rawFileData.ReadBytes(LevelFileSectionIdentifierHasher.NumberOfBytesForLevelSectionIdentifier);

        FileReadingException.ReaderAssert(
            sectionIdentifierBytes.SequenceEqual(sectionReader.GetSectionIdentifierBytes()),
            "Section Identifier mismatch!");

        sectionReader.ReadSection(_rawFileData, result);

        FileReadingException.ReaderAssert(
            interval.Start + interval.Length == _rawFileData.Position,
            "Byte reading mismatch!");
    }

    public void Dispose()
    {
    }
}