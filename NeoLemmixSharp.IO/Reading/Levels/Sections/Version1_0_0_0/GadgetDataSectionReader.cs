using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

internal sealed class GadgetDataSectionReader : LevelDataSectionReader
{
    private readonly StringIdLookup _stringIdLookup;

    public GadgetDataSectionReader(
        StringIdLookup stringIdLookup)
        : base(LevelFileSectionIdentifier.GadgetDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData, int numberOfItemsInSection)
    {
        levelData.AllGadgetData.Capacity = numberOfItemsInSection;

        while (numberOfItemsInSection-- > 0)
        {
            var newGadgetDatum = ReadNextGadgetData(rawFileData, levelData);
            levelData.AllGadgetData.Add(newGadgetDatum);
        }
    }

    private GadgetData ReadNextGadgetData(RawLevelFileDataReader rawFileData, LevelData levelData)
    {
        int styleId = rawFileData.Read16BitUnsignedInteger();
        int pieceId = rawFileData.Read16BitUnsignedInteger();

        int overrideNameId = rawFileData.Read16BitUnsignedInteger();

        int x = rawFileData.Read16BitUnsignedInteger();
        int y = rawFileData.Read16BitUnsignedInteger();

        x -= ReadWriteHelpers.PositionOffset;
        y -= ReadWriteHelpers.PositionOffset;

        int dhtByte = rawFileData.Read8BitUnsignedInteger();
        ReadWriteHelpers.AssertDihedralTransformationByteMakesSense(dhtByte);
        var dht = new DihedralTransformation(dhtByte);

        int initialStateId = rawFileData.Read8BitUnsignedInteger();
        var renderMode = GadgetRenderModeHelpers.GetEnumValue(rawFileData.Read8BitUnsignedInteger());

        var inputNames = ReadOverrideInputNames(rawFileData);

        var result = new GadgetData
        {
            Id = levelData.AllGadgetData.Count,
            OverrideName = _stringIdLookup[overrideNameId],

            StyleName = new StyleIdentifier(_stringIdLookup[styleId]),
            PieceName = new PieceIdentifier(_stringIdLookup[pieceId]),

            Position = new Point(x, y),

            InitialStateId = initialStateId,
            GadgetRenderMode = renderMode,

            Orientation = dht.Orientation,
            FacingDirection = dht.FacingDirection,

            OverrideInputNames = inputNames
        };


        ReadProperties(rawFileData, result);

        return result;
    }

    private GadgetInputData[] ReadOverrideInputNames(RawLevelFileDataReader rawFileData)
    {
        int numberOfInputNames = rawFileData.Read8BitUnsignedInteger();

        var result = CollectionsHelper.GetArrayForSize<GadgetInputData>(numberOfInputNames);

        for (var i = 0; i < result.Length; i++)
        {
            int inputNameStringId = rawFileData.Read16BitUnsignedInteger();
            var inputName = _stringIdLookup[inputNameStringId];
            result[i] = new GadgetInputData(inputName);
        }

        return result;
    }

    private static void ReadProperties(RawLevelFileDataReader rawFileData, GadgetData result)
    {
        int numberOfProperties = rawFileData.Read8BitUnsignedInteger();
        while (numberOfProperties-- > 0)
        {
            uint rawGadgetProperty = rawFileData.Read8BitUnsignedInteger();
            var gadgetProperty = GadgetPropertyHasher.GetEnumValue(rawGadgetProperty);
            int propertyValue = rawFileData.Read32BitSignedInteger();
            result.AddProperty(gadgetProperty, propertyValue);
        }
    }
}
