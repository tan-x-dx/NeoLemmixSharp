using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class AndGateBuilder : IGadgetBuilder
{
    public int GadgetBuilderId { get; }
    public SpriteData SpriteData { get; }

    public GadgetBase BuildGadget(GadgetSpriteBuilder gadgetSpriteBuilder, GadgetData gadgetData, LemmingManager lemmingManager)
    {
        throw new NotImplementedException();
    }
}
