using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.LevelIo.Data.Level.Gadgets;
using NeoLemmixSharp.Engine.LevelIo.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Styles.Gadgets;

public static class LogicGateGadgetReader
{
    public static GadgetArchetypeData ReadGadgetArchetypeData(
        string styleName,
        string pieceName,
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
