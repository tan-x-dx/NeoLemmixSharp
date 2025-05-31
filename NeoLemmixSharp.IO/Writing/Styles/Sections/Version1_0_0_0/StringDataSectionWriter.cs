using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.FileFormats;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Writing.Styles.Sections.Version1_0_0_0;

internal sealed class StringDataSectionWriter : StyleDataSectionWriter
{
    private readonly StringIdLookup _stringIdLookup;

    public StringDataSectionWriter(StringIdLookup stringIdLookup)
        : base(StyleFileSectionIdentifier.StringDataSection, true)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override ushort CalculateNumberOfItemsInSection(StyleData styleData)
    {
        GenerateStringIdLookup(styleData);

        return (ushort)_stringIdLookup.Count;
    }

    [SkipLocalsInit]
    public override void WriteSection(
        RawStyleFileDataWriter writer,
        StyleData levelData)
    {
        _stringIdLookup.WriteStrings(writer);
    }

    private void GenerateStringIdLookup(StyleData styleData)
    {
        FileWritingException.WriterAssert(_stringIdLookup.Count == 0, "Expected string id lookup to be empty!");


    }
}
