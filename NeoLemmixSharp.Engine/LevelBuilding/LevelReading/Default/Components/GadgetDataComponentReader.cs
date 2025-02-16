using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

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

        int initialStateId = rawFileData.Read8BitUnsignedInteger();
        var renderMode = GetGadgetRenderMode(rawFileData.Read8BitUnsignedInteger());

        byte orientationByte = rawFileData.Read8BitUnsignedInteger();
        LevelReadWriteHelpers.DecipherOrientationByte(orientationByte, out var orientation, out var facingDirection);

        var result = new GadgetData
        {
            Id = levelData.AllGadgetData.Count,

            Style = _stringIdLookup[styleId],
            GadgetPiece = _stringIdLookup[pieceId],

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