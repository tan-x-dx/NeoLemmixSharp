using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;

public interface IGadgetBuilder
{
    int GadgetBuilderId { get; }
    SpriteData SpriteData { get; }

    GadgetBase BuildGadget(
        GadgetSpriteBuilder gadgetSpriteBuilder,
        GadgetData gadgetData,
        IPerfectHasher<Lemming> lemmingHasher);
}

public interface IGadgetAnimationData : IGadgetBuilder
{
    IEnumerable<GadgetAnimationArchetypeData> AnimationArchetypes();
}