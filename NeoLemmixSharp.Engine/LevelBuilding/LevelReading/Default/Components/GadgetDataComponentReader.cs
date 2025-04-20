using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Components;

public sealed class GadgetDataComponentReader : ILevelDataReader
{
    private readonly List<string> _stringIdLookup;

    public bool AlreadyUsed { get; private set; }
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.GadgetDataSectionIdentifier;

    public GadgetDataComponentReader(
        Version version,
        List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public void ReadSection(RawFileData rawFileData, LevelData levelData)
    {
        AlreadyUsed = true;
        int numberOfItemsInSection = rawFileData.Read16BitUnsignedInteger();
        levelData.AllGadgetData.Capacity = numberOfItemsInSection;

        while (numberOfItemsInSection-- > 0)
        {
            var newGadgetDatum = ReadNextGadgetData(rawFileData, levelData);
            levelData.AllGadgetData.Add(newGadgetDatum);
        }
    }

    private GadgetData ReadNextGadgetData(RawFileData rawFileData, LevelData levelData)
    {
        int numberOfBytesToRead = rawFileData.Read16BitUnsignedInteger();
        int initialPosition = rawFileData.Position;

        int styleId = rawFileData.Read16BitUnsignedInteger();
        int pieceId = rawFileData.Read16BitUnsignedInteger();

        int x = rawFileData.Read16BitUnsignedInteger();
        int y = rawFileData.Read16BitUnsignedInteger();

        int dhtByte = rawFileData.Read8BitUnsignedInteger();
        LevelReadWriteHelpers.AssertDihedralTransformationByteMakesSense(dhtByte);
        var dht = new DihedralTransformation(dhtByte);

        int initialStateId = rawFileData.Read8BitUnsignedInteger();
        var renderMode = GadgetRenderModeHelpers.GetGadgetRenderMode(rawFileData.Read8BitUnsignedInteger());

        int numberOfInputNames = rawFileData.Read8BitUnsignedInteger();
        var inputNames = CollectionsHelper.GetArrayForSize<string>(numberOfInputNames);

        var result = new GadgetData
        {
            Id = levelData.AllGadgetData.Count,

            Style = _stringIdLookup[styleId],
            GadgetPiece = _stringIdLookup[pieceId],

            Position = new Point(x - LevelReadWriteHelpers.PositionOffset, y - LevelReadWriteHelpers.PositionOffset),

            InitialStateId = initialStateId,
            GadgetRenderMode = renderMode,

            Orientation = dht.Orientation,
            FacingDirection = dht.FacingDirection,

            InputNames = inputNames
        };

        ReadInputNames(rawFileData, inputNames, numberOfInputNames);
        ReadProperties(rawFileData, result);

        AssertGadgetDataBytesMakeSense(
            rawFileData.Position,
            initialPosition,
            numberOfBytesToRead);

        return result;
    }

    private void ReadInputNames(RawFileData rawFileData, string[] inputNames, int numberOfInputNames)
    {
        var i = 0;
        while (i < numberOfInputNames)
        {
            int inputNameStringId = rawFileData.Read16BitUnsignedInteger();
            inputNames[i++] = _stringIdLookup[inputNameStringId];
        }
    }

    private static void ReadProperties(RawFileData rawFileData, GadgetData result)
    {
        int numberOfProperties = rawFileData.Read8BitUnsignedInteger();
        while (numberOfProperties-- > 0)
        {
            var gadgetProperty = GadgetPropertyHelpers.GetGadgetProperty(rawFileData.Read8BitUnsignedInteger());
            int propertyValue = rawFileData.Read32BitSignedInteger();
            result.AddProperty(gadgetProperty, propertyValue);
        }
    }

    private static void AssertGadgetDataBytesMakeSense(
        int currentPosition,
        int initialPosition,
        int expectedByteCount)
    {
        if (currentPosition - initialPosition == expectedByteCount)
            return;

        throw new LevelReadingException(
            "Wrong number of bytes read for gadget data! " +
            $"Expected: {expectedByteCount}, Actual: {currentPosition - initialPosition}");
    }
}