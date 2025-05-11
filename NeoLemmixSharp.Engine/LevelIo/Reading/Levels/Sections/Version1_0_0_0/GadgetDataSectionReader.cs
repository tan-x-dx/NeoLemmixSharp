using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.LevelIo.Data.Level;
using NeoLemmixSharp.Engine.LevelIo.Data.Level.Gadgets;
using NeoLemmixSharp.Engine.LevelIo.Data.Style;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Sections.Version1_0_0_0;

public sealed class GadgetDataSectionReader : LevelDataSectionReader
{
    public override bool IsNecessary => false;

    private readonly List<string> _stringIdLookup;

    public GadgetDataSectionReader(
        List<string> stringIdLookup)
        : base(LevelFileSectionIdentifier.GadgetDataSection)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData)
    {
        int numberOfItemsInSection = rawFileData.Read16BitUnsignedInteger();
        levelData.AllGadgetData.Capacity = numberOfItemsInSection;

        while (numberOfItemsInSection-- > 0)
        {
            var newGadgetDatum = ReadNextGadgetData(rawFileData, levelData);
            levelData.AllGadgetData.Add(newGadgetDatum);
        }
    }

    private GadgetData ReadNextGadgetData(RawLevelFileDataReader rawFileData, LevelData levelData)
    {
        int numberOfBytesToRead = rawFileData.Read16BitUnsignedInteger();
        int initialPosition = rawFileData.Position;

        int styleId = rawFileData.Read16BitUnsignedInteger();
        int pieceId = rawFileData.Read16BitUnsignedInteger();

        int x = rawFileData.Read16BitUnsignedInteger();
        int y = rawFileData.Read16BitUnsignedInteger();

        x -= LevelReadWriteHelpers.PositionOffset;
        y -= LevelReadWriteHelpers.PositionOffset;

        int dhtByte = rawFileData.Read8BitUnsignedInteger();
        LevelReadWriteHelpers.AssertDihedralTransformationByteMakesSense(dhtByte);
        var dht = new DihedralTransformation(dhtByte);

        int initialStateId = rawFileData.Read8BitUnsignedInteger();
        var renderMode = GadgetRenderModeHelpers.GetEnumValue(rawFileData.Read8BitUnsignedInteger());

        int numberOfInputNames = rawFileData.Read8BitUnsignedInteger();
        var inputNames = CollectionsHelper.GetArrayForSize<string>(numberOfInputNames);

        var result = new GadgetData
        {
            Id = levelData.AllGadgetData.Count,

            Style = new StyleIdentifier(_stringIdLookup[styleId]),
            GadgetPiece = new PieceIdentifier(_stringIdLookup[pieceId]),

            Position = new Point(x, y),

            InitialStateId = initialStateId,
            GadgetRenderMode = renderMode,

            Orientation = dht.Orientation,
            FacingDirection = dht.FacingDirection,

            InputNames = inputNames
        };

        ReadInputNames(rawFileData, inputNames, numberOfInputNames);
        ReadProperties(rawFileData, result);

        FileReadingException.AssertBytesMakeSense(
            rawFileData.Position,
            initialPosition,
            numberOfBytesToRead,
            "gadget data");

        return result;
    }

    private void ReadInputNames(RawLevelFileDataReader rawFileData, string[] inputNames, int numberOfInputNames)
    {
        var i = 0;
        while (i < numberOfInputNames)
        {
            int inputNameStringId = rawFileData.Read16BitUnsignedInteger();
            inputNames[i++] = _stringIdLookup[inputNameStringId];
        }
    }

    private static void ReadProperties(RawLevelFileDataReader rawFileData, GadgetData result)
    {
        int numberOfProperties = rawFileData.Read8BitUnsignedInteger();
        while (numberOfProperties-- > 0)
        {
            uint rawGadgetProperty = rawFileData.Read8BitUnsignedInteger();
            var gadgetProperty = GadgetPropertyHelpers.GetEnumValue(rawGadgetProperty);
            int propertyValue = rawFileData.Read32BitSignedInteger();
            result.AddProperty(gadgetProperty, propertyValue);
        }
    }
}