using NeoLemmixSharp.Engine.LevelBuilding.Gadgets;
using NeoLemmixSharp.Engine.LevelIo.Data.Gadgets;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default.Styles.Gadgets;

public static class LogicGateGadgetReader
{
    public static LogicGateArchetypeBuilder ReadGadget(
        string styleName,
        string pieceName,
        LogicGateType logicGateType,
        RawFileData rawFileData)
    {
        return new LogicGateArchetypeBuilder
        {
            StyleName = styleName,
            PieceName = pieceName,
            LogicGateType = logicGateType,

            SpriteData = null!
        };
    }
}
