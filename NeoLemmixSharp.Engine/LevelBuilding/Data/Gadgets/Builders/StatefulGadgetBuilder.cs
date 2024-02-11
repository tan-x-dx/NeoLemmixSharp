using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class StatefulGadgetBuilder : IGadgetBuilder
{
    private readonly List<GadgetState> _stateList = new();

    public required int GadgetBuilderId { get; init; }
    
    public GadgetBase BuildGadget(
        GadgetData gadgetData, 
        IPerfectHasher<Lemming> lemmingHasher)
    {


        return new StatefulGadget(
            gadgetData.Id,
            GadgetBehaviour.AllItems[1],
            gadgetData.Orientation,
            new RectangularLevelRegion(0,0,0,0),
            new HitBox(new RectangularLevelRegion(0,0,0,0), new ILemmingFilter[0]),
            _stateList.ToArray(),
            new ItemTracker<Lemming>(lemmingHasher));
    }
}