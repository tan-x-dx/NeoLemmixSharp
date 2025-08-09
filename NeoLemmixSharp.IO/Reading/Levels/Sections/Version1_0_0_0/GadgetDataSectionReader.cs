using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using NeoLemmixSharp.IO.Data.Style.Theme;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Util;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

internal sealed class GadgetDataSectionReader : LevelDataSectionReader
{
    private readonly FileReaderStringIdLookup _stringIdLookup;

    public GadgetDataSectionReader(
        FileReaderStringIdLookup stringIdLookup)
        : base(LevelFileSectionIdentifier.GadgetDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileDataReader reader, LevelData levelData, int numberOfItemsInSection)
    {
        levelData.AllGadgetData.Capacity = numberOfItemsInSection;

        while (numberOfItemsInSection-- > 0)
        {
            var newGadgetDatum = ReadGadgetData(reader, levelData);
            levelData.AllGadgetData.Add(newGadgetDatum);
        }
    }

    private GadgetData ReadGadgetData(RawLevelFileDataReader reader, LevelData levelData)
    {
        int gadgetId = reader.Read16BitUnsignedInteger();

        FileReadingException.ReaderAssert(gadgetId == levelData.AllGadgetData.Count + 1, "GadgetData id mismatch!");

        int styleId = reader.Read16BitUnsignedInteger();
        int pieceId = reader.Read16BitUnsignedInteger();

        int overrideNameId = reader.Read16BitUnsignedInteger();

        int positionData = reader.Read32BitSignedInteger();
        var position = ReadWriteHelpers.DecodePoint(positionData);

        int dhtByte = reader.Read8BitUnsignedInteger();
        ReadWriteHelpers.AssertDihedralTransformationByteMakesSense(dhtByte);
        var dht = new DihedralTransformation(dhtByte);

        int initialStateId = reader.Read8BitUnsignedInteger();
        var renderMode = GadgetRenderModeHelpers.GetEnumValue(reader.Read8BitUnsignedInteger());

        var inputNames = ReadOverrideInputNames(reader);
        var layerColorData = ReadLayerColorData(reader);
        var overrideHitBoxCriteriaData = ReadOverrideHitBoxCriteriaData(reader);

        var result = new GadgetData
        {
            Identifier = new GadgetIdentifier(gadgetId),
            OverrideName = _stringIdLookup[overrideNameId],

            StyleIdentifier = new StyleIdentifier(_stringIdLookup[styleId]),
            PieceIdentifier = new PieceIdentifier(_stringIdLookup[pieceId]),

            Position = position,

            InitialStateId = initialStateId,
            GadgetRenderMode = renderMode,

            Orientation = dht.Orientation,
            FacingDirection = dht.FacingDirection,

            // OverrideInputNames = inputNames,
            LayerColorData = layerColorData,
            // OverrideHitBoxCriteriaData = overrideHitBoxCriteriaData
        };

        ReadProperties(reader, result);

        AssertGadgetInputDataIsConsistent(result);

        return result;
    }

    private GadgetTriggerName[] ReadOverrideInputNames(RawLevelFileDataReader reader)
    {
        int numberOfInputNames = reader.Read8BitUnsignedInteger();

        var result = Helpers.GetArrayForSize<GadgetTriggerName>(numberOfInputNames);

        for (var i = 0; i < result.Length; i++)
        {
            int inputNameStringId = reader.Read16BitUnsignedInteger();
            var inputName = _stringIdLookup[inputNameStringId];
            result[i] = new GadgetTriggerName(inputName);
        }

        return result;
    }

    private static GadgetLayerColorData[] ReadLayerColorData(RawLevelFileDataReader reader)
    {
        int numberOfColorData = reader.Read8BitUnsignedInteger();

        var result = Helpers.GetArrayForSize<GadgetLayerColorData>(numberOfColorData);

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = ReadLayerColorDatum(reader);
        }

        return result;
    }

    private static GadgetLayerColorData ReadLayerColorDatum(RawLevelFileDataReader reader)
    {
        int stateIndex = reader.Read8BitUnsignedInteger();
        int layerIndex = reader.Read8BitUnsignedInteger();

        bool usesSpecificColor = reader.ReadBool();

        if (usesSpecificColor)
        {
            var colorBytes = reader.ReadBytes(4);
            var color = ReadWriteHelpers.ReadArgbBytes(colorBytes);
            return new GadgetLayerColorData(stateIndex, layerIndex, color);
        }

        int tribeId = reader.Read8BitUnsignedInteger();
        uint rawTribeSpriteLayerColorType = reader.Read8BitUnsignedInteger();
        var spriteLayerColorType = TribeSpriteLayerColorTypeHelpers.GetEnumValue(rawTribeSpriteLayerColorType);

        return new GadgetLayerColorData(stateIndex, layerIndex, tribeId, spriteLayerColorType);
    }

    private static HitBoxCriteriaData? ReadOverrideHitBoxCriteriaData(RawLevelFileDataReader reader)
    {
        bool hasOverrideHitBoxCriteriaData = reader.ReadBool();

        return hasOverrideHitBoxCriteriaData
            ? new GadgetHitBoxCriteriaReader<RawLevelFileDataReader>(reader).ReadHitBoxCriteria()
            : null;
    }

    private static void ReadProperties(RawLevelFileDataReader reader, GadgetData result)
    {
        int numberOfProperties = reader.Read8BitUnsignedInteger();
        while (numberOfProperties-- > 0)
        {
            uint rawGadgetProperty = reader.Read8BitUnsignedInteger();
            var gadgetProperty = GadgetPropertyHasher.GetEnumValue(rawGadgetProperty);
            int propertyValue = reader.Read32BitSignedInteger();
            result.AddProperty(gadgetProperty, propertyValue);
        }
    }

    private static void AssertGadgetInputDataIsConsistent(GadgetData result)
    {
        if (!result.TryGetProperty(GadgetProperty.NumberOfInputs, out var numberOfInputsSpecified))
            numberOfInputsSpecified = 0;

        //  var numberOfOverrideInputNames = result.OverrideInputNames.Length;

        //  FileReadingException.ReaderAssert(numberOfInputsSpecified == numberOfOverrideInputNames, "Mismatch between number of inputs specified and number of override input names!");
    }
}
