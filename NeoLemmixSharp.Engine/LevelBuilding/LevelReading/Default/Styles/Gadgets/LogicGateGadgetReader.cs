using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Styles.Gadgets;

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
