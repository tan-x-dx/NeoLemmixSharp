﻿using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Writing.Styles.Sections.Version1_0_0_0;

internal sealed class StringDataSectionWriter : StyleDataSectionWriter
{
    private readonly MutableStringIdLookup _stringIdLookup;

    public StringDataSectionWriter(MutableStringIdLookup stringIdLookup)
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
        _stringIdLookup.RecordString(styleData.Identifier);
        _stringIdLookup.RecordString(styleData.Name);
        _stringIdLookup.RecordString(styleData.Author);
        _stringIdLookup.RecordString(styleData.Description);
        _stringIdLookup.RecordString(styleData.ThemeData.LemmingSpriteData.LemmingSpriteStyleIdentifier);
    }

    private void RecordTerrainArchetypeDataStrings(StyleData styleData)
    {
        foreach (var kvp in styleData.TerrainArchetypeData)
        {
            var terrainArchetypeData = kvp.Value;
            _stringIdLookup.RecordString(terrainArchetypeData.PieceIdentifier);
            _stringIdLookup.RecordString(terrainArchetypeData.Name);
        }
    }

    private void RecordGadgetArchetypeDataStrings(StyleData styleData)
    {
        foreach (var kvp in styleData.GadgetArchetypeData)
        {
            var gadgetArchetypeDatum = kvp.Value;
            _stringIdLookup.RecordString(gadgetArchetypeDatum.PieceIdentifier);
            _stringIdLookup.RecordString(gadgetArchetypeDatum.GadgetName);

            foreach (var gadgetStateData in gadgetArchetypeDatum.AllGadgetStateData)
            {
                _stringIdLookup.RecordString(gadgetStateData.StateName);
            }
        }
    }
}
