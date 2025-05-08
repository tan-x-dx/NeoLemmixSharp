using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelIo.Data;
using NeoLemmixSharp.Engine.LevelIo.Versions;
using NeoLemmixSharp.Engine.LevelIo.Writing.Levels.Sections;

namespace NeoLemmixSharp.Engine.LevelIo.Writing.Levels;

public readonly ref struct LevelWriter
{
    private readonly LevelData _levelData;
    private readonly FileVersion _version;

    public LevelWriter(LevelData levelData, FileVersion version)
    {
        _levelData = levelData;
        _version = version;
    }

    public void WriteToFile(string filePath)
    {
        var writer = new RawLevelFileDataWriter();

        var sectionWriters = VersionHelper.Instance.GetLevelDataSectionWritersForVersion(_version);

        foreach (var sectionWriter in sectionWriters)
        {
            WriteSection(writer, sectionWriter);
        }

        writer.WriteToFile(filePath, _version);
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
        writer.Write(sectionWriter.GetSectionIdentifierBytes());
        writer.Write(numberOfItemsInSection);

        sectionWriter.WriteSection(writer, _levelData);
        writer.EndWritingSection(sectionWriter.SectionIdentifier);
    }
}