using NeoLemmixSharp.Engine.Level.Gadgets;
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

        throw new LevelReadingException("Could not locate gadget data within style file!");
    }

    private static IGadgetArchetypeBuilder ReadGadgetArchetypeData(
        string styleName,
        string pieceName,
        RawFileData rawFileData)
    {
        int numberOfBytesToRead = rawFileData.Read8BitUnsignedInteger();
        int initialBytesRead = rawFileData.BytesRead;

        var basicGadgetType = (GadgetType)rawFileData.Read8BitUnsignedInteger();

        var result = basicGadgetType switch
        {
            GadgetType.HitBoxGadget => HitBoxGadgetReader.ReadGadget(styleName, pieceName, rawFileData),
            GadgetType.HatchGadget => HatchGadgetReader.ReadGadget(styleName, pieceName, rawFileData),
            GadgetType.GadgetMover => throw new NotImplementedException(),
            GadgetType.GadgetResizer => throw new NotImplementedException(),
            GadgetType.GadgetStateChanger => throw new NotImplementedException(),
            GadgetType.AndGate => AndGateGadgetReader.ReadGadget(styleName, pieceName, rawFileData),
            GadgetType.OrGate => throw new NotImplementedException(),
            GadgetType.NotGate => throw new NotImplementedException(),
            GadgetType.XorGate => throw new NotImplementedException(),

            _ => ThrowUnknownGadgetTypeException(basicGadgetType)
        };

        AssertGadgetArchetypeBytesMakeSense(
            rawFileData.BytesRead,
            initialBytesRead,
            numberOfBytesToRead);

        return result;
    }

    [DoesNotReturn]
    private static IGadgetArchetypeBuilder ThrowUnknownGadgetTypeException(GadgetType basicGadgetType)
    {
        throw new ArgumentOutOfRangeException(nameof(basicGadgetType), basicGadgetType, "Unknown gadget type");
    }

    private static void AssertGadgetArchetypeBytesMakeSense(
        int bytesRead,
        int initialBytesRead,
        int numberOfBytesToRead)
    {
        if (bytesRead - initialBytesRead == numberOfBytesToRead)
            return;

        throw new LevelReadingException(
            "Wrong number of bytes read for gadget archetype data section! " +
            $"Expected: {numberOfBytesToRead}, Actual: {bytesRead - initialBytesRead}");
    }
}
