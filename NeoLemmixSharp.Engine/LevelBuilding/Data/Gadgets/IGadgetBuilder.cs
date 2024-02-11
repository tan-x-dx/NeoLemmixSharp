using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;

public interface IGadgetBuilder
{
    int GadgetBuilderId { get; }

    GadgetBase BuildGadget(
        GadgetData gadgetData,
        IPerfectHasher<Lemming> lemmingHasher);
}