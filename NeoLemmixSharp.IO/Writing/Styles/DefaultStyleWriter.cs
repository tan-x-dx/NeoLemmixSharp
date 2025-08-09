using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Versions;
using NeoLemmixSharp.IO.Writing.Styles.Sections;

namespace NeoLemmixSharp.IO.Writing.Styles;

internal readonly ref struct DefaultStyleWriter
{
    private readonly StyleData _styleData;
    private readonly FileFormatVersion _version;

    public DefaultStyleWriter(StyleData styleData, FileFormatVersion version)
    {
        _styleData = styleData;
        _version = version;
    }

    public void WriteStyleData(Stream stream)
    {
        using var writer = new RawStyleFileDataWriter();

        var sectionWriters = VersionHelper.GetStyleDataSectionWritersForVersion(_version);

        foreach (var sectionWriter in sectionWriters)
        {
            WriteSection(writer, sectionWriter);
        }

        writer.WriteToFile(stream, _version);
    }

    private void WriteSection(
        RawStyleFileDataWriter writer,
        StyleDataSectionWriter sectionWriter)
    {
        var numberOfItemsInSection = sectionWriter.CalculateNumberOfItemsInSection(_styleData);
        if (numberOfItemsInSection == 0)
        {
            FileWritingException.WriterAssert(
                !sectionWriter.IsNecessary,
                "No data for necessary section!");
            return;
        }

        writer.BeginWritingSection(sectionWriter.SectionIdentifier);
        writer.Write16BitUnsignedInteger(sectionWriter.GetSectionIdentifier());
        writer.Write16BitUnsignedInteger(numberOfItemsInSection);

        sectionWriter.WriteSection(writer, _styleData);
        writer.EndWritingSection(sectionWriter.SectionIdentifier);
    }
}
