using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Components;

public sealed class GadgetDataComponentReader : ILevelDataReader
{
    private readonly Dictionary<int, int> _gadgetBuilderIdLookup = new();
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

        while (numberOfItemsInSection-- > 0)
        {
            var newGadgetDatum = ReadNextGadgetData(rawFileData, levelData);
            levelData.AllGadgetData.Add(newGadgetDatum);
        }

        ProcessGadgetBuilders(levelData);
    }

    private GadgetData ReadNextGadgetData(RawFileData rawFileData, LevelData levelData)
    {
        int numberOfBytesToRead = rawFileData.Read16BitUnsignedInteger();
        int initialBytesRead = rawFileData.BytesRead;

        int styleId = rawFileData.Read16BitUnsignedInteger();
        int pieceId = rawFileData.Read16BitUnsignedInteger();

        var gadgetBuilderId = RegisterGadgetBuilderId(styleId, pieceId);

        int x = rawFileData.Read16BitUnsignedInteger();
        int y = rawFileData.Read16BitUnsignedInteger();

        int initialStateId = rawFileData.Read8BitUnsignedInteger();
        var renderMode = GetGadgetRenderMode(rawFileData.Read8BitUnsignedInteger());

        byte orientationByte = rawFileData.Read8BitUnsignedInteger();
        var (orientation, facingDirection) = LevelReadWriteHelpers.DecipherOrientationByte(orientationByte);

        var result = new GadgetData
        {
            Id = levelData.AllGadgetData.Count,

            Style = _stringIdLookup[styleId],
            GadgetPiece = _stringIdLookup[pieceId],
            GadgetBuilderId = gadgetBuilderId,

            X = x - LevelReadWriteHelpers.PositionOffset,
            Y = y - LevelReadWriteHelpers.PositionOffset,

            InitialStateId = initialStateId,
            GadgetRenderMode = renderMode,

            Orientation = orientation,
            FacingDirection = facingDirection,
        };

        int numberOfProperties = rawFileData.Read8BitUnsignedInteger();
        while (numberOfProperties-- > 0)
        {
            var gadgetProperty = GetGadgetProperty(rawFileData.Read8BitUnsignedInteger());
            int propertyValue = rawFileData.Read32BitSignedInteger();
            result.AddProperty(gadgetProperty, propertyValue);
        }

        AssertGadgetDataBytesMakeSense(
            rawFileData.BytesRead,
            initialBytesRead,
            numberOfBytesToRead);

        return result;
    }

    private int RegisterGadgetBuilderId(
        int styleStringId,
        int pieceStringId)
    {
        var gadgetArchetypeIdLookupKey = (styleStringId << 16) | pieceStringId;

        ref var gadgetArchetypeDataId = ref CollectionsMarshal.GetValueRefOrAddDefault(_gadgetBuilderIdLookup, gadgetArchetypeIdLookupKey, out var exists);
        if (!exists)
        {
            gadgetArchetypeDataId = _gadgetBuilderIdLookup.Count - 1;
        }
        return gadgetArchetypeDataId;
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
        int bytesRead,
        int initialBytesRead,
        int numberOfBytesToRead)
    {
        if (bytesRead - initialBytesRead == numberOfBytesToRead)
            return;

        throw new LevelReadingException(
            "Wrong number of bytes read for gadget data! " +
            $"Expected: {numberOfBytesToRead}, Actual: {bytesRead - initialBytesRead}");
    }

    private void ProcessGadgetBuilders(LevelData levelData)
    {
        levelData.AllGadgetBuilders.EnsureCapacity(_gadgetBuilderIdLookup.Count);

        foreach (var (styleAndPiece, gadgetBuilderId) in _gadgetBuilderIdLookup)
        {
            var styleStringId = (styleAndPiece >> 16) & 0xff;
            var pieceStringId = styleAndPiece & 0xff;

            var style = _stringIdLookup[styleStringId];
            var piece = _stringIdLookup[pieceStringId];
        }

    }
}