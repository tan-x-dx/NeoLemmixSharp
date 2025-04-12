using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders.ArchetypeData;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Styles.Gadgets;

public static class HitBoxGadgetReader
{
    public static HitBoxGadgetArchetypeBuilder ReadGadget(string styleName, string pieceName, RawFileData rawFileData)
    {
        var gadgetStateData = new GadgetStateArchetypeData[0];

        var spriteData = new SpriteArchetypeData
        {
            BaseSpriteSize = new Size(),
            NumberOfLayers = 1,
            MaxNumberOfFrames = 1,
            SpriteArchetypeDataForStates = []
        };

        var result = new HitBoxGadgetArchetypeBuilder
        {
            StyleName = styleName,
            PieceName = pieceName,

            ResizeType = ResizeType.None,

            AllGadgetStateData = gadgetStateData,
            SpriteData = spriteData
        };

        return result;
    }
}
