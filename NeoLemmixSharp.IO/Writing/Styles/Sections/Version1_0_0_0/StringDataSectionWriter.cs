using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.FileFormats;

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
        PopulateStringIdLookup(styleData);

        return (ushort)_stringIdLookup.Count;
    }

    public override void WriteSection(
        RawStyleFileDataWriter writer,
        StyleData levelData)
    {
        _stringIdLookup.WriteStrings(writer);
    }

    private void PopulateStringIdLookup(StyleData styleData)
    {
        FileWritingException.WriterAssert(_stringIdLookup.Count == 0, "Expected string id lookup to be empty!");

        RecordThemeDataStrings(styleData);
        RecordTerrainArchetypeDataStrings(styleData);
        RecordGadgetArchetypeDataStrings(styleData);
    }

    private void RecordThemeDataStrings(StyleData styleData)
    {
        _stringIdLookup.RecordString(styleData.Identifier.ToString());
        _stringIdLookup.RecordString(styleData.Name);
        _stringIdLookup.RecordString(styleData.Author);
        _stringIdLookup.RecordString(styleData.Description);
        _stringIdLookup.RecordString(styleData.ThemeData.LemmingSpriteData.LemmingSpriteStyleIdentifier.ToString());
    }

    private void RecordTerrainArchetypeDataStrings(StyleData styleData)
    {
        foreach (var kvp in styleData.TerrainArchetypeData)
        {
            var pieceIdentifier = kvp.Key;
            _stringIdLookup.RecordString(pieceIdentifier.ToString());
        }
    }

    private void RecordGadgetArchetypeDataStrings(StyleData styleData)
    {
        foreach (var kvp in styleData.GadgetArchetypeData)
        {
            var pieceIdentifier = kvp.Key;
            var gadgetArchetypeDatum = kvp.Value;
            _stringIdLookup.RecordString(pieceIdentifier.ToString());

            foreach (var gadgetStateData in gadgetArchetypeDatum.AllGadgetStateData)
            {
                _stringIdLookup.RecordString(gadgetStateData.StateName);
            }
        }
    }
}
