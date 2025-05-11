using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.LevelIo.Data.Level.Gadgets;
using NeoLemmixSharp.Engine.LevelIo.Data.Style;
using NeoLemmixSharp.Engine.LevelIo.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Styles.Sections.Version1_0_0_0;

public sealed class GadgetArchetypeDataSectionReader : StyleDataSectionReader
{
    private readonly List<string> _stringIdLookup;

    public override bool IsNecessary => false;

    public GadgetArchetypeDataSectionReader(List<string> stringIdLookup)
        : base(StyleFileSectionIdentifier.GadgetArchetypeDataSection)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawStyleFileDataReader rawFileData, StyleData styleData)
    {
        int numberOfItemsInSection = rawFileData.Read16BitUnsignedInteger();
        styleData.GadgetArchetypeData.EnsureCapacity(numberOfItemsInSection);

        while (numberOfItemsInSection-- > 0)
        {
            var newGadgetArchetypeDatum = ReadNextGadgetArchetypeData(styleData.Identifier, rawFileData);
            styleData.GadgetArchetypeData.Add(newGadgetArchetypeDatum.PieceName, newGadgetArchetypeDatum);
        }
    }

    private GadgetArchetypeData ReadNextGadgetArchetypeData(
        StyleIdentifier styleName,
        RawStyleFileDataReader rawFileData)
    {
        int numberOfBytesToRead = rawFileData.Read16BitUnsignedInteger();
        int initialPosition = rawFileData.Position;

        int pieceId = rawFileData.Read16BitUnsignedInteger();
        var pieceName = new PieceIdentifier(_stringIdLookup[pieceId]);

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

            _ => Helpers.ThrowUnknownEnumValueException<GadgetType, GadgetArchetypeData>(gadgetType)
        };

        FileReadingException.AssertBytesMakeSense(
            rawFileData.Position,
            initialPosition,
            numberOfBytesToRead,
            "gadget archetype data section");

        return result;
    }
}
