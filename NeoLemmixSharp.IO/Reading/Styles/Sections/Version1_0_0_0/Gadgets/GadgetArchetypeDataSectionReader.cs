using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
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

    public override void ReadSection(RawStyleFileDataReader reader, StyleData styleData, int numberOfItemsInSection)
    {
        styleData.GadgetArchetypeDataLookup.EnsureCapacity(numberOfItemsInSection);

        while (numberOfItemsInSection-- > 0)
        {
            var newGadgetArchetypeDatum = ReadGadgetArchetypeData(reader, styleData.Identifier);
            styleData.GadgetArchetypeDataLookup.Add(newGadgetArchetypeDatum.PieceIdentifier, newGadgetArchetypeDatum);
        }
    }

    private IGadgetArchetypeData ReadGadgetArchetypeData(
        RawStyleFileDataReader reader,
        StyleIdentifier styleIdentifier)
    {
        int pieceId = reader.Read16BitUnsignedInteger();
        var pieceName = new PieceIdentifier(_stringIdLookup[pieceId]);

        int nameId = reader.Read16BitUnsignedInteger();
        var gadgetName = new GadgetName(_stringIdLookup[nameId]);

        var gadgetType = (GadgetType)reader.Read8BitUnsignedInteger();

        return gadgetType switch
        {
            GadgetType.HitBoxGadget => ReadHitBoxGadgetArchetypeData(reader, styleIdentifier, pieceName, gadgetName),
            GadgetType.HatchGadget => throw new NotImplementedException(),
            GadgetType.AndGate => throw new NotImplementedException(),
            GadgetType.OrGate => throw new NotImplementedException(),
            GadgetType.NotGate => throw new NotImplementedException(),
            GadgetType.XorGate => throw new NotImplementedException(),

            _ => Helpers.ThrowUnknownEnumValueException<GadgetType, IGadgetArchetypeData>(gadgetType),
        };

        /*= GadgetTypeHelpers.GetEnumValue(rawGadgetType);

         uint rawResizeType = reader.Read8BitUnsignedInteger();
         var resizeType = ReadWriteHelpers.DecodeResizeType(rawResizeType);

         int baseWidth = reader.Read16BitUnsignedInteger();
         int baseHeight = reader.Read16BitUnsignedInteger();
         var baseSpriteSize = new Size(baseWidth, baseHeight);

         var nineSliceData = ReadNineSliceData(reader, baseSpriteSize);

         var gadgetStates = ReadGadgetStates(reader);

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

         ReadMiscData(reader, result);

         return result;*/
    }

    private HitBoxGadgetArchetypeData ReadHitBoxGadgetArchetypeData(RawStyleFileDataReader reader, StyleIdentifier styleIdentifier, PieceIdentifier pieceIdentifier, GadgetName gadgetName)
    {
        uint rawResizeType = reader.Read8BitUnsignedInteger();
        var resizeType = ReadWriteHelpers.DecodeResizeType(rawResizeType);

        int baseWidth = reader.Read16BitUnsignedInteger();
        int baseHeight = reader.Read16BitUnsignedInteger();
        var baseSpriteSize = new Size(baseWidth, baseHeight);

        var nineSliceData = ReadNineSliceData(reader, baseSpriteSize);

        var gadgetStates = ReadGadgetStates(reader);

        return new HitBoxGadgetArchetypeData
        {
            StyleIdentifier = styleIdentifier,
            PieceIdentifier = pieceIdentifier,
            GadgetName = gadgetName,

            ResizeType = resizeType,
            BaseSpriteSize = baseSpriteSize,
            NineSliceData = nineSliceData,
            GadgetStates = []//gadgetStates
        };
    }

    private static RectangularRegion ReadNineSliceData(
        RawStyleFileDataReader reader,
        Size baseSpriteSize)
    {
        int nineSliceLeft = reader.Read16BitUnsignedInteger();
        int nineSliceWidth = reader.Read16BitUnsignedInteger();

        int nineSliceTop = reader.Read16BitUnsignedInteger();
        int nineSliceHeight = reader.Read16BitUnsignedInteger();

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

    private GadgetStateArchetypeData[] ReadGadgetStates(RawStyleFileDataReader reader)
    {
        var gadgetStateReader = new GadgetStateReader(reader, _stringIdLookup);

        int numberOfGadgetStates = reader.Read8BitUnsignedInteger();
        var result = Helpers.GetArrayForSize<GadgetStateArchetypeData>(numberOfGadgetStates);

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = gadgetStateReader.ReadStateData();
        }

        return result;
    }

    private static void ReadMiscData(RawStyleFileDataReader reader, IGadgetArchetypeData result)
    {
        int numberOfProperties = reader.Read8BitUnsignedInteger();
        while (numberOfProperties-- > 0)
        {
            uint rawGadgetProperty = reader.Read8BitUnsignedInteger();
            var gadgetProperty = GadgetArchetypeMiscDataTypeHasher.GetEnumValue(rawGadgetProperty);
            int propertyValue = reader.Read32BitSignedInteger();
            //  result.AddMiscData(gadgetProperty, propertyValue);
        }
    }
}
