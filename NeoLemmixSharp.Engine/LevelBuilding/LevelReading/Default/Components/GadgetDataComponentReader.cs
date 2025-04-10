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

        int orientationByte = rawFileData.Read8BitUnsignedInteger();
        var dht = new DihedralTransformation(orientationByte);

        int initialStateId = rawFileData.Read8BitUnsignedInteger();
        var renderMode = GetGadgetRenderMode(rawFileData.Read8BitUnsignedInteger());

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

        var i = 0;
        while (i < numberOfInputNames)
        {
            int inputNameStringId = rawFileData.Read16BitUnsignedInteger();
            inputNames[i++] = _stringIdLookup[inputNameStringId];
        }

        Debug.Assert(i == inputNames.Length);

        int numberOfProperties = rawFileData.Read8BitUnsignedInteger();
        while (numberOfProperties-- > 0)
        {
            var gadgetProperty = GetGadgetProperty(rawFileData.Read8BitUnsignedInteger());
            int propertyValue = rawFileData.Read32BitSignedInteger();
            result.AddProperty(gadgetProperty, propertyValue);
        }

        AssertGadgetDataBytesMakeSense(
            rawFileData.Position,
            initialPosition,
            numberOfBytesToRead);

        return result;
    }

    private static GadgetRenderMode GetGadgetRenderMode(int rawValue)
    {
        var enumValue = (GadgetRenderMode)rawValue;

        return enumValue switch
        {
            GadgetRenderMode.NoRender => GadgetRenderMode.NoRender,
            GadgetRenderMode.BehindTerrain => GadgetRenderMode.BehindTerrain,
            GadgetRenderMode.InFrontOfTerrain => GadgetRenderMode.InFrontOfTerrain,
            GadgetRenderMode.OnlyOnTerrain => GadgetRenderMode.OnlyOnTerrain,

            _ => throw new LevelReadingException(
                $"Invalid GadgetRenderMode! Value: {rawValue}")
        };
    }

    private static GadgetProperty GetGadgetProperty(int rawValue)
    {
        var enumValue = (GadgetProperty)rawValue;

        return enumValue switch
        {
            GadgetProperty.HatchGroupId => GadgetProperty.HatchGroupId,
            GadgetProperty.TeamId => GadgetProperty.TeamId,
            GadgetProperty.SkillId => GadgetProperty.SkillId,
            GadgetProperty.Width => GadgetProperty.Width,
            GadgetProperty.Height => GadgetProperty.Height,
            GadgetProperty.RawLemmingState => GadgetProperty.RawLemmingState,
            GadgetProperty.Count => GadgetProperty.Count,
            GadgetProperty.InitialAnimationFrame => GadgetProperty.InitialAnimationFrame,
            GadgetProperty.LogicGateType => GadgetProperty.LogicGateType,

            _ => throw new LevelReadingException(
                $"Invalid GadgetProperty! Value: {rawValue}")
        };
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