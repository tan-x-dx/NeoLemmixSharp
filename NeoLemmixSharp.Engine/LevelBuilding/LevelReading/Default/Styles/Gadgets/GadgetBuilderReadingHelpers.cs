using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets.BinaryLogic;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Styles.Gadgets;

public static class GadgetBuilderReadingHelpers
{
    public static IGadgetArchetypeBuilder GetGadgetBuilderData(
        string styleName,
        string pieceName,
        RawFileData rawFileData,
        bool pieceExists)
    {
        if (pieceExists)
            return ReadGadgetArchetypeData(
                styleName,
                pieceName,
                rawFileData);

        return ThrowGadgetNotFoundException();
    }

    [DoesNotReturn]
    private static IGadgetArchetypeBuilder ThrowGadgetNotFoundException()
    {
        throw new LevelReadingException("Could not locate gadget data within style file!");
    }

    private static IGadgetArchetypeBuilder ReadGadgetArchetypeData(
        string styleName,
        string pieceName,
        RawFileData rawFileData)
    {
        int numberOfBytesToRead = rawFileData.Read8BitUnsignedInteger();
        int initialPosition = rawFileData.Position;

        var basicGadgetType = (GadgetType)rawFileData.Read8BitUnsignedInteger();

        var result = basicGadgetType switch
        {
            GadgetType.HitBoxGadget => HitBoxGadgetReader.ReadGadget(styleName, pieceName, rawFileData),
            GadgetType.HatchGadget => HatchGadgetReader.ReadGadget(styleName, pieceName, rawFileData),
            GadgetType.GadgetMover => throw new NotImplementedException(),
            GadgetType.GadgetResizer => throw new NotImplementedException(),
            GadgetType.GadgetStateChanger => throw new NotImplementedException(),
            GadgetType.AndGate => LogicGateGadgetReader.ReadGadget(styleName, pieceName, LogicGateType.AndGate, rawFileData),
            GadgetType.OrGate => LogicGateGadgetReader.ReadGadget(styleName, pieceName, LogicGateType.OrGate, rawFileData),
            GadgetType.NotGate => LogicGateGadgetReader.ReadGadget(styleName, pieceName, LogicGateType.NotGate, rawFileData),
            GadgetType.XorGate => LogicGateGadgetReader.ReadGadget(styleName, pieceName, LogicGateType.XorGate, rawFileData),

            _ => ThrowUnknownGadgetTypeException(basicGadgetType)
        };

        AssertGadgetArchetypeBytesMakeSense(
            rawFileData.Position,
            initialPosition,
            numberOfBytesToRead);

        return result;
    }

    [DoesNotReturn]
    private static IGadgetArchetypeBuilder ThrowUnknownGadgetTypeException(GadgetType basicGadgetType)
    {
        throw new ArgumentOutOfRangeException(nameof(basicGadgetType), basicGadgetType, "Unknown gadget type");
    }

    private static void AssertGadgetArchetypeBytesMakeSense(
        int currentPosition,
        int initialPosition,
        int expectedByteCount)
    {
        if (currentPosition - initialPosition == expectedByteCount)
            return;

        throw new LevelReadingException(
            "Wrong number of bytes read for gadget archetype data section! " +
            $"Expected: {expectedByteCount}, Actual: {currentPosition - initialPosition}");
    }
}
