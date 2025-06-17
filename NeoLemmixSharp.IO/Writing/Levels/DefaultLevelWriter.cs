using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Versions;
using NeoLemmixSharp.IO.Writing.Levels.Sections;

namespace NeoLemmixSharp.IO.Writing.Levels;

public readonly ref struct DefaultLevelWriter
{
    private readonly LevelData _levelData;
    private readonly FileFormatVersion _version;

    public DefaultLevelWriter(LevelData levelData, FileFormatVersion version)
    {
        _levelData = levelData;
        _version = version;
    }

    public void WriteToFile(Stream stream)
    {
        using var writer = new RawLevelFileDataWriter();

        var sectionWriters = VersionHelper.GetLevelDataSectionWritersForVersion(_version);

        foreach (var sectionWriter in sectionWriters)
        {
            WriteSection(writer, sectionWriter);
        }

        writer.WriteToFile(stream, _version);
    }

    private void WriteSection(
        RawLevelFileDataWriter writer,
        LevelDataSectionWriter sectionWriter)
    {
        var numberOfItemsInSection = sectionWriter.CalculateNumberOfItemsInSection(_levelData);
        if (numberOfItemsInSection == 0)
        {
            FileWritingException.WriterAssert(
                !sectionWriter.IsNecessary,
                "No data for necessary section!");
            return;
        }

        writer.BeginWritingSection(sectionWriter.SectionIdentifier);
        writer.WriteBytes(sectionWriter.GetSectionIdentifierBytes());
        writer.Write(numberOfItemsInSection);

        sectionWriter.WriteSection(writer, _levelData);
        writer.EndWritingSection(sectionWriter.SectionIdentifier);
    }
}
