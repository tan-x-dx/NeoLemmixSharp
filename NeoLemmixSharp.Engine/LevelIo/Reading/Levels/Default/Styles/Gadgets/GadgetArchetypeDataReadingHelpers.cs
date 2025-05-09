using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.LevelIo.Data.Gadgets;
using NeoLemmixSharp.Engine.LevelIo.Data.Gadgets.ArchetypeData;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Default.Styles.Gadgets;

public static class GadgetArchetypeDataReadingHelpers
{
    public static GadgetArchetypeData GetGadgetArchetypeData(
        string styleName,
        string pieceName,
        RawStyleFileDataReader rawFileData,
        [DoesNotReturnIf(false)] bool pieceExists)
    {
        if (pieceExists)
            return ReadGadgetArchetypeData(
                styleName,
                pieceName,
                rawFileData);

        return ThrowGadgetNotFoundException();
    }

    [DoesNotReturn]
    private static GadgetArchetypeData ThrowGadgetNotFoundException()
    {
        throw new FileReadingException("Could not locate gadget data within style file!");
    }

    private static GadgetArchetypeData ReadGadgetArchetypeData(
        string styleName,
        string pieceName,
        RawStyleFileDataReader rawFileData)
    {
        int numberOfBytesToRead = rawFileData.Read8BitUnsignedInteger();
        int initialPosition = rawFileData.Position;

        int rawGadgetType = rawFileData.Read8BitUnsignedInteger();
        var gadgetType = (GadgetType)rawGadgetType;

        var result = gadgetType switch
        {
            GadgetType.HitBoxGadget => HitBoxGadgetReader.ReadGadgetArchetypeData(styleName, pieceName, gadgetType, rawFileData),
            GadgetType.HatchGadget => HatchGadgetReader.ReadGadgetArchetypeData(styleName, pieceName, gadgetType, rawFileData),
            GadgetType.GadgetMover => throw new NotImplementedException(),
            GadgetType.GadgetResizer => throw new NotImplementedException(),
            GadgetType.GadgetStateChanger => throw new NotImplementedException(),
            GadgetType.AndGate => LogicGateGadgetReader.ReadGadgetArchetypeData(styleName, pieceName, gadgetType, LogicGateType.AndGate, rawFileData),
            GadgetType.OrGate => LogicGateGadgetReader.ReadGadgetArchetypeData(styleName, pieceName, gadgetType, LogicGateType.OrGate, rawFileData),
            GadgetType.NotGate => LogicGateGadgetReader.ReadGadgetArchetypeData(styleName, pieceName, gadgetType, LogicGateType.NotGate, rawFileData),
            GadgetType.XorGate => LogicGateGadgetReader.ReadGadgetArchetypeData(styleName, pieceName, gadgetType, LogicGateType.XorGate, rawFileData),

            _ => Helpers.ThrowUnknownEnumValueException<GadgetType, GadgetArchetypeData>(rawGadgetType)
        };

        FileReadingException.AssertBytesMakeSense(
            rawFileData.Position,
            initialPosition,
            numberOfBytesToRead,
            "gadget archetype data section");

        return result;
    }
}
