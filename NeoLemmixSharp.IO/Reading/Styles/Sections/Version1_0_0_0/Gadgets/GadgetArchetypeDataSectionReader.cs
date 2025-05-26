using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections.Version1_0_0_0.Gadgets;

internal sealed class GadgetArchetypeDataSectionReader : StyleDataSectionReader
{
    private readonly StringIdLookup _stringIdLookup;

    public GadgetArchetypeDataSectionReader(StringIdLookup stringIdLookup)
        : base(StyleFileSectionIdentifier.GadgetArchetypeDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawStyleFileDataReader rawFileData, StyleData styleData, int numberOfItemsInSection)
    {
        styleData.GadgetArchetypeData.EnsureCapacity(numberOfItemsInSection);

        while (numberOfItemsInSection-- > 0)
        {
            var newGadgetArchetypeDatum = ReadNextGadgetArchetypeData(styleData.Identifier, rawFileData);
            styleData.GadgetArchetypeData.Add(newGadgetArchetypeDatum.PieceName, newGadgetArchetypeDatum);
        }
    }

    private GadgetArchetypeData ReadNextGadgetArchetypeData(
        StyleIdentifier styleName,
        RawStyleFileDataReader rawFileData)
    {
        int pieceId = rawFileData.Read16BitUnsignedInteger();
        var pieceName = new PieceIdentifier(_stringIdLookup[pieceId]);

        uint rawGadgetType = rawFileData.Read8BitUnsignedInteger();
        var gadgetType = GadgetTypeHelpers.GetEnumValue(rawGadgetType);

        uint rawResizeType = rawFileData.Read8BitUnsignedInteger();
        var resizeType = ReadWriteHelpers.DecodeResizeType(rawResizeType);

        var inputNames = ReadInputNames(rawFileData);
        var gadgetStates = ReadGadgetStates(rawFileData);

        var result = new GadgetArchetypeData
        {
            StyleName = styleName,
            PieceName = pieceName,

            GadgetType = gadgetType,
            ResizeType = resizeType,

            AllGadgetStateData = gadgetStates,
            AllGadgetInputs = inputNames,
        };

        return result;
    }

    private GadgetInputData[] ReadInputNames(RawStyleFileDataReader rawFileData)
    {
        int numberOfInputNames = rawFileData.Read8BitUnsignedInteger();

        var result = CollectionsHelper.GetArrayForSize<GadgetInputData>(numberOfInputNames);

        var i = 0;
        while (i < numberOfInputNames)
        {
            int inputNameStringId = rawFileData.Read16BitUnsignedInteger();
            var inputName = _stringIdLookup[inputNameStringId];
            result[i++] = new GadgetInputData(inputName);
        }

        return result;
    }

    private GadgetStateArchetypeData[] ReadGadgetStates(RawStyleFileDataReader rawFileData)
    {
        int numberOfGadgetStates = rawFileData.Read8BitUnsignedInteger();

        var result = CollectionsHelper.GetArrayForSize<GadgetStateArchetypeData>(numberOfGadgetStates);

        var gadgetStateReader = new GadgetStateReader(rawFileData);
        var i = 0;
        while (i < numberOfGadgetStates)
        {
            result[i++] = gadgetStateReader.ReadStateData();
        }

        return result;
    }
}
