using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.Data.Level.Gadget.Functional;
using NeoLemmixSharp.IO.Data.Level.Gadget.HatchGadget;
using NeoLemmixSharp.IO.Data.Level.Gadget.HitBoxGadget;
using NeoLemmixSharp.IO.Data.Level.Gadget.LogicGateGadget;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;
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

        IGadgetInstanceSpecificationData gadgetInstanceTypeData = gadgetType switch
        {
            GadgetType.HitBoxGadget => ReadHitBoxGadgetTypeDatum(reader),
            GadgetType.HatchGadget => ReadHatchGadgetTypeDatum(reader),
            GadgetType.LogicGate => ReadLogicGateGadgetTypeDatum(reader),
            GadgetType.LevelTimerObserver => ReadLevelTimerObserverGadgetTypeDatum(reader),

            _ => Helpers.ThrowUnknownEnumValueException<GadgetType, IGadgetInstanceSpecificationData>(gadgetType),
        };

        return new GadgetInstanceData
        {
            Identifier = new GadgetIdentifier(gadgetId),
            OverrideName = _stringIdLookup[overrideNameId],
            StyleIdentifier = _stringIdLookup[styleId],
            PieceIdentifier = _stringIdLookup[pieceId],
            Position = position,
            GadgetRenderMode = renderMode,
            Orientation = dht.Orientation,
            FacingDirection = dht.FacingDirection,
            IsFastForward = isFastForward,

            SpecificationData = gadgetInstanceTypeData
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

    private HitBoxGadgetInstanceSpecifcationData ReadHitBoxGadgetTypeDatum(RawLevelFileDataReader reader)
    {
        return new HitBoxGadgetInstanceSpecifcationData()
        {
            InitialStateId = 0,
            GadgetStates = []
        };
    }

    private HatchGadgetInstanceSpecificationData ReadHatchGadgetTypeDatum(RawLevelFileDataReader reader)
    {
        int hatchGroupId = reader.Read8BitUnsignedInteger();
        int tribeId = reader.Read8BitUnsignedInteger();
        FileReadingException.ReaderAssert(tribeId < EngineConstants.MaxNumberOfTribes, "Invalid tribe ID!");
        uint rawStateData = reader.Read32BitUnsignedInteger();
        int numberOfLemmingsToRelease = reader.Read16BitUnsignedInteger();
        FileReadingException.ReaderAssert(numberOfLemmingsToRelease < EngineConstants.MaxNumberOfLemmings, "Too many lemmings specified!");

        return new HatchGadgetInstanceSpecificationData()
        {
            InitialStateId = 0,
            GadgetStates = [],
            HatchGroupId = hatchGroupId,
            TribeId = tribeId,
            RawStateData = rawStateData,
            NumberOfLemmingsToRelease = numberOfLemmingsToRelease,
        };
    }

    private LogicGateGadgetInstanceSpecificationData ReadLogicGateGadgetTypeDatum(RawLevelFileDataReader reader)
    {
        uint rawGadgetType = reader.Read8BitUnsignedInteger();
        LogicGateGadgetType logicGateGadgetType = LogicGateGadgetTypeHelpers.GetEnumValue(rawGadgetType);

        int numberOfInputs = reader.Read8BitUnsignedInteger();
        FileReadingException.ReaderAssert(numberOfInputs <= EngineConstants.MaxAllowedNumberOfGadgetTriggers, "Too many triggers specified for logic gate!");
        AssertNumberOfInputsMakesSense();

        return new LogicGateGadgetInstanceSpecificationData()
        {
            LogicGateGadgetType = logicGateGadgetType,
            NumberOfInputs = numberOfInputs
        };

        void AssertNumberOfInputsMakesSense()
        {
            switch (logicGateGadgetType)
            {
                case LogicGateGadgetType.AndGate: FileReadingException.ReaderAssert(numberOfInputs >= 2, "Less than two inputs specified for AND gate!"); break;
                case LogicGateGadgetType.OrGate: FileReadingException.ReaderAssert(numberOfInputs >= 2, "Less than two inputs specified for OR gate!"); break;
                case LogicGateGadgetType.NotGate: FileReadingException.ReaderAssert(numberOfInputs == 1, "Expected ONE input for NOT gate!"); break;
                case LogicGateGadgetType.XorGate: FileReadingException.ReaderAssert(numberOfInputs == 2, "Less TWO inputs for XOR gate!"); break;
            }
        }
    }

    private static LevelTimerObserverGadgetInstanceSpecificationData ReadLevelTimerObserverGadgetTypeDatum(RawLevelFileDataReader reader)
    {
        uint rawObservationType = reader.Read8BitUnsignedInteger();
        LevelTimerObservationType observationType = LevelTimerObservationTypeHelpers.GetEnumValue(rawObservationType);
        uint rawComparisonType = reader.Read8BitUnsignedInteger();
        ComparisonType comparisonType = ComparisonTypeHelpers.GetEnumValue(rawComparisonType);
        int requiredValue = reader.Read16BitUnsignedInteger();

        return new LevelTimerObserverGadgetInstanceSpecificationData()
        {
            ObservationType = observationType,
            ComparisonType = comparisonType,
            RequiredValue = requiredValue,
        };
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
