using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Functional;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class HatchGadgetBuilder : IGadgetBuilder
{
    public int GadgetBuilderId { get; }

    public GadgetBase BuildGadget(GadgetData gadgetData, IPerfectHasher<Lemming> lemmingHasher)
    {
        throw new NotImplementedException();
        //return new HatchGadget();
    }
}