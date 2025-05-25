using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections.Version1_0_0_0;

internal static class LogicGateGadgetReader
{
    public static GadgetArchetypeData ReadGadgetArchetypeData(
        StyleIdentifier styleName,
        PieceIdentifier pieceName,
        GadgetType gadgetType,
        LogicGateType logicGateType,
        RawStyleFileDataReader rawFileData)
    {
        return new GadgetArchetypeData
        {
            StyleName = styleName,
            PieceName = pieceName,
            GadgetType = gadgetType,
            ResizeType = Common.ResizeType.None,

            AllGadgetStateData = [],
            AllGadgetInputs = []
        };
    }
}
