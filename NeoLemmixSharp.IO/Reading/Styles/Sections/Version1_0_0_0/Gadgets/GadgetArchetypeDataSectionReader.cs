using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Util;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections.Version1_0_0_0.Gadgets;

internal sealed class GadgetArchetypeDataSectionReader : StyleDataSectionReader
{
    private readonly FileReaderStringIdLookup _stringIdLookup;

    public GadgetArchetypeDataSectionReader(FileReaderStringIdLookup stringIdLookup)
        : base(StyleFileSectionIdentifier.GadgetArchetypeDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawStyleFileDataReader rawFileData, StyleData styleData, int numberOfItemsInSection)
    {
        styleData.GadgetArchetypeData.EnsureCapacity(numberOfItemsInSection);

        while (numberOfItemsInSection-- > 0)
        {
            var newGadgetArchetypeDatum = ReadGadgetArchetypeData(styleData.Identifier, rawFileData);
            styleData.GadgetArchetypeData.Add(newGadgetArchetypeDatum.PieceIdentifier, newGadgetArchetypeDatum);
        }
    }

    private GadgetArchetypeData ReadGadgetArchetypeData(
        StyleIdentifier styleName,
        RawStyleFileDataReader rawFileData)
    {
        int pieceId = rawFileData.Read16BitUnsignedInteger();
        var pieceName = new PieceIdentifier(_stringIdLookup[pieceId]);

        int nameId = rawFileData.Read16BitUnsignedInteger();
        var gadgetName = _stringIdLookup[nameId];

        uint rawGadgetType = rawFileData.Read8BitUnsignedInteger();
        var gadgetType = GadgetTypeHelpers.GetEnumValue(rawGadgetType);

        uint rawResizeType = rawFileData.Read8BitUnsignedInteger();
        var resizeType = ReadWriteHelpers.DecodeResizeType(rawResizeType);

        int baseWidth = rawFileData.Read16BitUnsignedInteger();
        int baseHeight = rawFileData.Read16BitUnsignedInteger();
        var baseSpriteSize = new Size(baseWidth, baseHeight);

        var nineSliceData = ReadNineSliceData(rawFileData, baseSpriteSize);

        var gadgetStates = ReadGadgetStates(rawFileData);

        GadgetArchetypeValidation.AssertGadgetStateDataMakesSense(gadgetType, gadgetStates);

        var result = new GadgetArchetypeData
        {
            GadgetName = gadgetName,
            StyleIdentifier = styleName,
            PieceIdentifier = pieceName,

            GadgetType = gadgetType,
            ResizeType = resizeType,

            BaseSpriteSize = baseSpriteSize,
            NineSliceData = nineSliceData,

            AllGadgetStateData = gadgetStates
        };

        ReadMiscData(rawFileData, result);

        GadgetArchetypeValidation.AssertGadgetArchetypeDataHasRequiredMiscData(result);

        return result;
    }

    private static RectangularRegion ReadNineSliceData(
        RawStyleFileDataReader rawFileData,
        Size baseSpriteSize)
    {
        int nineSliceLeft = rawFileData.Read16BitUnsignedInteger();
        int nineSliceWidth = rawFileData.Read16BitUnsignedInteger();

        int nineSliceTop = rawFileData.Read16BitUnsignedInteger();
        int nineSliceHeight = rawFileData.Read16BitUnsignedInteger();

        FileReadingException.ReaderAssert(nineSliceLeft >= 0, "Invalid nine slice definition!");
        FileReadingException.ReaderAssert(nineSliceWidth >= 1, "Invalid nine slice definition!");
        FileReadingException.ReaderAssert(nineSliceLeft + nineSliceWidth <= baseSpriteSize.W, "Invalid nine slice definition!");

        FileReadingException.ReaderAssert(nineSliceTop >= 0, "Invalid nine slice definition!");
        FileReadingException.ReaderAssert(nineSliceHeight >= 1, "Invalid nine slice definition!");
        FileReadingException.ReaderAssert(nineSliceTop + nineSliceHeight <= baseSpriteSize.H, "Invalid nine slice definition!");

        var p = new Point(nineSliceLeft, nineSliceTop);
        var s = new Size(nineSliceWidth, nineSliceHeight);
        return new RectangularRegion(p, s);
    }

    private GadgetStateArchetypeData[] ReadGadgetStates(RawStyleFileDataReader rawFileData)
    {
        var gadgetStateReader = new GadgetStateReader(rawFileData, _stringIdLookup);

        int numberOfGadgetStates = rawFileData.Read8BitUnsignedInteger();
        var result = Helpers.GetArrayForSize<GadgetStateArchetypeData>(numberOfGadgetStates);

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = gadgetStateReader.ReadStateData();
        }

        return result;
    }

    private static void ReadMiscData(RawStyleFileDataReader rawFileData, GadgetArchetypeData result)
    {
        int numberOfProperties = rawFileData.Read8BitUnsignedInteger();
        while (numberOfProperties-- > 0)
        {
            uint rawGadgetProperty = rawFileData.Read8BitUnsignedInteger();
            var gadgetProperty = GadgetArchetypeMiscDataTypeHasher.GetEnumValue(rawGadgetProperty);
            int propertyValue = rawFileData.Read32BitSignedInteger();
            result.AddMiscData(gadgetProperty, propertyValue);
        }
    }
}
