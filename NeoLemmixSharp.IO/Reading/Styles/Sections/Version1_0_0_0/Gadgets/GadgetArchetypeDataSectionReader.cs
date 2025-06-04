using NeoLemmixSharp.Common;
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

        int numberOfLayers = rawFileData.Read8BitUnsignedInteger();
        int numberOfFrames = rawFileData.Read8BitUnsignedInteger();

        var gadgetStates = ReadGadgetStates(rawFileData);

        AssertGadgetStateDataMakesSense(gadgetType, gadgetStates);

        var result = new GadgetArchetypeData
        {
            GadgetName = gadgetName,
            StyleIdentifier = styleName,
            PieceIdentifier = pieceName,

            GadgetType = gadgetType,
            ResizeType = resizeType,

            BaseSpriteSize = baseSpriteSize,
            NineSliceData = nineSliceData,
            MaxNumberOfFrames = numberOfFrames,
            NumberOfLayers = numberOfLayers,

            AllGadgetStateData = gadgetStates
        };

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
        var result = CollectionsHelper.GetArrayForSize<GadgetStateArchetypeData>(numberOfGadgetStates);

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = gadgetStateReader.ReadStateData();
        }

        return result;
    }

    private static void AssertGadgetStateDataMakesSense(
        GadgetType gadgetType,
        GadgetStateArchetypeData[] gadgetStates)
    {
        var baseGadgetType = gadgetType.GetBaseGadgetType();

        switch (baseGadgetType)
        {
            case BaseGadgetType.HitBox:
                return;

            case BaseGadgetType.Hatch:
                AssertHatchGadgetStateDataMakesSense();
                return;

            case BaseGadgetType.Functional:
                AssertFunctionalGadgetStateDataMakesSense();
                return;
        }

        return;

        void AssertHatchGadgetStateDataMakesSense()
        {
            FileReadingException.ReaderAssert(gadgetStates.Length == 2, "Expected exactly 2 states for Hatch gadget!");
        }

        void AssertFunctionalGadgetStateDataMakesSense()
        {
            FileReadingException.ReaderAssert(gadgetStates.Length == EngineConstants.NumberOfAllowedStatesForFunctionalGadgets, "Expected exactly 2 states for Functional gadget!");
        }
    }
}
