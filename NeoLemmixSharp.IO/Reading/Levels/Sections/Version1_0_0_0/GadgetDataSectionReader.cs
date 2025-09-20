using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.Data.Level.Gadget.HatchGadget;
using NeoLemmixSharp.IO.Data.Level.Gadget.HitBoxGadget;
using NeoLemmixSharp.IO.Data.Level.Gadget.LogicGateGadget;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
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
        levelData.AllGadgetInstanceData.Capacity = numberOfItemsInSection;

        while (numberOfItemsInSection-- > 0)
        {
            var newGadgetDatum = ReadGadgetInstanceDatum(reader, levelData);
            levelData.AllGadgetInstanceData.Add(newGadgetDatum);
        }
    }

    private GadgetInstanceData ReadGadgetInstanceDatum(RawLevelFileDataReader reader, LevelData levelData)
    {
        int gadgetId = reader.Read16BitUnsignedInteger();

        FileReadingException.ReaderAssert(gadgetId == levelData.AllGadgetInstanceData.Count + 1, "GadgetData id mismatch!");

        int styleId = reader.Read16BitUnsignedInteger();
        int pieceId = reader.Read16BitUnsignedInteger();

        var gadgetType = (GadgetType)reader.Read8BitUnsignedInteger();

        int overrideNameId = reader.Read16BitUnsignedInteger();

        int positionData = reader.Read32BitSignedInteger();
        Point position = ReadWriteHelpers.DecodePoint(positionData);

        int dhtByte = reader.Read8BitUnsignedInteger();
        ReadWriteHelpers.AssertDihedralTransformationByteMakesSense(dhtByte);
        DihedralTransformation dht = new(dhtByte);

        bool isFastForward = reader.ReadBool();

        GadgetRenderMode renderMode = GadgetRenderModeHelpers.GetEnumValue(reader.Read8BitUnsignedInteger());

        IGadgetTypeInstanceData gadgetInstanceTypeData = gadgetType switch
        {
            GadgetType.HitBoxGadget => ReadHitBoxGadgetTypeDatum(reader),
            GadgetType.HatchGadget => ReadHatchGadgetTypeDatum(reader),
            GadgetType.LogicGate => ReadLogicGateGadgetTypeDatum(reader),
            GadgetType.LevelTimerObserver => ReadLevelTimerObserverGadgetTypeDatum(reader),

            _ => Helpers.ThrowUnknownEnumValueException<GadgetType, IGadgetTypeInstanceData>(gadgetType),
        };

        return new GadgetInstanceData
        {
            Identifier = new GadgetIdentifier(gadgetId),
            OverrideName = new GadgetName(_stringIdLookup[overrideNameId]),
            StyleIdentifier = new StyleIdentifier(_stringIdLookup[styleId]),
            PieceIdentifier = new PieceIdentifier(_stringIdLookup[pieceId]),
            Position = position,
            GadgetRenderMode = renderMode,
            Orientation = dht.Orientation,
            FacingDirection = dht.FacingDirection,
            IsFastForward = isFastForward,

            GadgetTypeInstanceData = gadgetInstanceTypeData
        };

        //var result = new GadgetData
        //{
        //    Identifier = new GadgetIdentifier(gadgetId),
        //    OverrideName = _stringIdLookup[overrideNameId],

        //    StyleIdentifier = new StyleIdentifier(_stringIdLookup[styleId]),
        //    PieceIdentifier = new PieceIdentifier(_stringIdLookup[pieceId]),

        //    Position = position,

        //    InitialStateId = initialStateId,
        //    GadgetRenderMode = renderMode,

        //    Orientation = dht.Orientation,
        //    FacingDirection = dht.FacingDirection,

        //    // OverrideInputNames = inputNames,
        //    LayerColorData = layerColorData,
        //    // OverrideHitBoxCriteriaData = overrideHitBoxCriteriaData
        //};

        //ReadProperties(reader, result);

        //return result;
    }

    private HitBoxGadgetTypeInstanceData ReadHitBoxGadgetTypeDatum(RawLevelFileDataReader reader)
    {
        var layerColorData = ReadLayerColorData(reader);

        return new HitBoxGadgetTypeInstanceData()
        {
            InitialStateId = 0,
            GadgetStates = [],
            LayerColorData = layerColorData,
        };
    }

    private HatchGadgetTypeInstanceData ReadHatchGadgetTypeDatum(RawLevelFileDataReader reader)
    {

        return new HatchGadgetTypeInstanceData()
        {
            InitialStateId = 0,
            GadgetStates = [],
            HatchGroupId = 0,
            TribeId = 0,
            RawStateData = 0,
            NumberOfLemmingsToRelease = 0,
        };
    }

    private LogicGateGadgetTypeInstanceData ReadLogicGateGadgetTypeDatum(RawLevelFileDataReader reader)
    {
        throw new NotImplementedException();
    }

    private IGadgetTypeInstanceData ReadLevelTimerObserverGadgetTypeDatum(RawLevelFileDataReader reader)
    {
        throw new NotImplementedException();
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

    /* private static void ReadProperties(RawLevelFileDataReader reader, GadgetData result)
     {
         int numberOfProperties = reader.Read8BitUnsignedInteger();
         while (numberOfProperties-- > 0)
         {
             uint rawGadgetProperty = reader.Read8BitUnsignedInteger();
             var gadgetProperty = GadgetPropertyTypeHasher.GetEnumValue(rawGadgetProperty);
             int propertyValue = reader.Read32BitSignedInteger();
             result.AddProperty(gadgetProperty, propertyValue);
         }
     }*/
}
