using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.LevelIo.Data.Level.Gadgets;
using NeoLemmixSharp.Engine.LevelIo.Data.Style;
using NeoLemmixSharp.Engine.LevelIo.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Styles.Sections.Version1_0_0_0;

public static class LogicGateGadgetReader
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
            SpriteData = null!
        };
    }
}
