using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;

public interface IGadgetBuilder
{
    int GadgetBuilderId { get; }
    SpriteData SpriteData { get; }

    GadgetBase BuildGadget(
        GadgetSpriteBankBuilder gadgetSpriteBankBuilder,
        GadgetData gadgetData,
        IPerfectHasher<Lemming> lemmingHasher);
}