using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.LevelIo.Data.Gadgets;
using NeoLemmixSharp.Engine.LevelIo.Data.Gadgets.ArchetypeData;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Default.Styles.Gadgets;

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
