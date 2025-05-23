﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style;
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

        int x = rawFileData.Read16BitUnsignedInteger();
        int y = rawFileData.Read16BitUnsignedInteger();

        x -= ReadWriteHelpers.PositionOffset;
        y -= ReadWriteHelpers.PositionOffset;

        int dhtByte = rawFileData.Read8BitUnsignedInteger();
        ReadWriteHelpers.AssertDihedralTransformationByteMakesSense(dhtByte);
        var dht = new DihedralTransformation(dhtByte);

        int initialStateId = rawFileData.Read8BitUnsignedInteger();
        var renderMode = GadgetRenderModeHelpers.GetEnumValue(rawFileData.Read8BitUnsignedInteger());

        int numberOfInputNames = rawFileData.Read8BitUnsignedInteger();
        var inputNames = CollectionsHelper.GetArrayForSize<string>(numberOfInputNames);

        var result = new GadgetData
        {
            Id = levelData.AllGadgetData.Count,

            StyleName = new StyleIdentifier(_stringIdLookup[styleId]),
            PieceName = new PieceIdentifier(_stringIdLookup[pieceId]),

            Position = new Point(x, y),

            InitialStateId = initialStateId,
            GadgetRenderMode = renderMode,

            Orientation = dht.Orientation,
            FacingDirection = dht.FacingDirection,

            InputNames = inputNames
        };

        ReadInputNames(rawFileData, inputNames, numberOfInputNames);
        ReadProperties(rawFileData, result);

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
            var gadgetProperty = GadgetPropertyHasher.GetEnumValue(rawGadgetProperty);
            int propertyValue = rawFileData.Read32BitSignedInteger();
            result.AddProperty(gadgetProperty, propertyValue);
        }
    }
}