﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using NeoLemmixSharp.IO.Data.Style.Theme;
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
            var newGadgetDatum = ReadGadgetData(rawFileData, levelData);
            levelData.AllGadgetData.Add(newGadgetDatum);
        }
    }

    private GadgetData ReadGadgetData(RawLevelFileDataReader rawFileData, LevelData levelData)
    {
        int gadgetId = rawFileData.Read16BitUnsignedInteger();

        FileReadingException.ReaderAssert(gadgetId == levelData.AllGadgetData.Count + 1, "GadgetData id mismatch!");

        int styleId = rawFileData.Read16BitUnsignedInteger();
        int pieceId = rawFileData.Read16BitUnsignedInteger();

        int overrideNameId = rawFileData.Read16BitUnsignedInteger();

        int positionData = rawFileData.Read32BitSignedInteger();
        var position = ReadWriteHelpers.DecodePoint(positionData);

        int dhtByte = rawFileData.Read8BitUnsignedInteger();
        ReadWriteHelpers.AssertDihedralTransformationByteMakesSense(dhtByte);
        var dht = new DihedralTransformation(dhtByte);

        int initialStateId = rawFileData.Read8BitUnsignedInteger();
        var renderMode = GadgetRenderModeHelpers.GetEnumValue(rawFileData.Read8BitUnsignedInteger());

        var inputNames = ReadOverrideInputNames(rawFileData);
        var layerColorData = ReadLayerColorData(rawFileData);
        var overrideHitBoxCriteriaData = ReadOverrideHitBoxCriteriaData(rawFileData);

        var result = new GadgetData
        {
            Id = gadgetId,
            OverrideName = _stringIdLookup[overrideNameId],

            StyleIdentifier = new StyleIdentifier(_stringIdLookup[styleId]),
            PieceIdentifier = new PieceIdentifier(_stringIdLookup[pieceId]),

            Position = position,

            InitialStateId = initialStateId,
            GadgetRenderMode = renderMode,

            Orientation = dht.Orientation,
            FacingDirection = dht.FacingDirection,

            OverrideInputNames = inputNames,
            LayerColorData = layerColorData,
            OverrideHitBoxCriteriaData = overrideHitBoxCriteriaData
        };

        ReadProperties(rawFileData, result);

        AssertGadgetInputDataIsConsistent(result);

        return result;
    }

    private GadgetInputName[] ReadOverrideInputNames(RawLevelFileDataReader rawFileData)
    {
        int numberOfInputNames = rawFileData.Read8BitUnsignedInteger();

        var result = Helpers.GetArrayForSize<GadgetInputName>(numberOfInputNames);

        for (var i = 0; i < result.Length; i++)
        {
            int inputNameStringId = rawFileData.Read16BitUnsignedInteger();
            var inputName = _stringIdLookup[inputNameStringId];
            result[i] = new GadgetInputName(inputName);
        }

        return result;
    }

    private static GadgetLayerColorData[] ReadLayerColorData(RawLevelFileDataReader rawFileData)
    {
        int numberOfColorData = rawFileData.Read8BitUnsignedInteger();

        var result = Helpers.GetArrayForSize<GadgetLayerColorData>(numberOfColorData);

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = ReadLayerColorDatum(rawFileData);
        }

        return result;
    }

    private static GadgetLayerColorData ReadLayerColorDatum(RawLevelFileDataReader rawFileData)
    {
        int stateIndex = rawFileData.Read8BitUnsignedInteger();
        int layerIndex = rawFileData.Read8BitUnsignedInteger();

        bool usesSpecificColor = rawFileData.ReadBool();

        if (usesSpecificColor)
        {
            var colorBytes = rawFileData.ReadBytes(4);
            var color = ReadWriteHelpers.ReadArgbBytes(colorBytes);
            return new GadgetLayerColorData(stateIndex, layerIndex, color);
        }

        int tribeId = rawFileData.Read8BitUnsignedInteger();
        uint rawTribeSpriteLayerColorType = rawFileData.Read8BitUnsignedInteger();
        var spriteLayerColorType = TribeSpriteLayerColorTypeHelpers.GetEnumValue(rawTribeSpriteLayerColorType);

        return new GadgetLayerColorData(stateIndex, layerIndex, tribeId, spriteLayerColorType);
    }

    private static HitBoxCriteriaData? ReadOverrideHitBoxCriteriaData(RawLevelFileDataReader rawFileData)
    {
        bool hasOverrideHitBoxCriteriaData = rawFileData.ReadBool();

        return hasOverrideHitBoxCriteriaData
            ? new GadgetHitBoxCriteriaReader<RawLevelFileDataReader>(rawFileData).ReadHitBoxCriteria()
            : null;
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

    private static void AssertGadgetInputDataIsConsistent(GadgetData result)
    {
        if (!result.TryGetProperty(GadgetProperty.NumberOfInputs, out var numberOfInputsSpecified))
            numberOfInputsSpecified = 0;

        var numberOfOverrideInputNames = result.OverrideInputNames.Length;

        FileReadingException.ReaderAssert(numberOfInputsSpecified == numberOfOverrideInputNames, "Mismatch between number of inputs specified and number of override input names!");
    }
}
