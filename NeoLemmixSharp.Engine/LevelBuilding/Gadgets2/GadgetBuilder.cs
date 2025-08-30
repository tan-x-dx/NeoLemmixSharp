using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.Engine.LevelBuilding.Gadgets2.HitBoxGadgets;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Level.Gadgets.Hatch;
using NeoLemmixSharp.IO.Data.Level.Gadgets.HitBox;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Hatch;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets2;

public sealed class GadgetBuilder
{
    /// <summary>
    /// Assumption: Each gadget instance will have about this many triggers defined.
    /// </summary>
    private const int GadgetTriggerCapacityMultiplier = 4;
    /// <summary>
    /// Assumption: Each gadget instance will have about this many behvaviours defined.
    /// </summary>
    private const int GadgetBehaviourCapacityMultiplier = 4;

    private readonly LevelData _levelData;
    private readonly List<GadgetBase> _gadgets;
    private readonly List<GadgetTrigger> _gadgetTriggers;
    private readonly List<GadgetBehaviour> _gadgetBehaviours;

    private readonly HitBoxGadgetBuilder _hitBoxGadgetBuilder;

    public GadgetBuilder(LevelData levelData)
    {
        _levelData = levelData;

        var numberOfGadgetInstances = _levelData.AllGadgetInstanceData.Count;

        _gadgets = new List<GadgetBase>(numberOfGadgetInstances);
        _gadgetTriggers = new List<GadgetTrigger>(numberOfGadgetInstances * GadgetTriggerCapacityMultiplier);
        _gadgetBehaviours = new List<GadgetBehaviour>(numberOfGadgetInstances * GadgetBehaviourCapacityMultiplier);

        _hitBoxGadgetBuilder = new HitBoxGadgetBuilder(_gadgetTriggers, _gadgetBehaviours);
    }

    public void BuildLevelGadgets(LemmingManager lemmingManager, TribeManager tribeManager)
    {
        var gadgetArchetypeDataLookup = StyleCache.GetAllGadgetArchetypeData(_levelData);

        foreach (var gadgetInstanceData in _levelData.AllGadgetInstanceData)
        {
            var stylePiecePair = new StylePiecePair(gadgetInstanceData.StyleIdentifier, gadgetInstanceData.PieceIdentifier);
            var gadgetArchetypeData = gadgetArchetypeDataLookup[stylePiecePair];

            if (gadgetArchetypeData.GadgetType != gadgetInstanceData.GadgetType)
                throw new Exception("GadgetType mismatch!");

            var newGadget = BuildGadget(gadgetArchetypeData, gadgetInstanceData, lemmingManager, tribeManager);
            _gadgets.Add(newGadget);
        }
    }

    private GadgetBase BuildGadget(IGadgetArchetypeData gadgetArchetypeData, IGadgetInstanceData gadgetInstanceData, LemmingManager lemmingManager, TribeManager tribeManager)
    {
        return gadgetArchetypeData switch
        {
            HitBoxGadgetArchetypeData hitBoxGadgetArchetypeData => _hitBoxGadgetBuilder.BuildHitBoxGadget(hitBoxGadgetArchetypeData, (HitBoxGadgetInstanceData)gadgetInstanceData, lemmingManager, tribeManager),
            HatchGadgetArchetypeData hatchGadgetArchetypeData => BuildHatchGadget(hatchGadgetArchetypeData, (HatchGadgetInstanceData)gadgetInstanceData, lemmingManager, tribeManager),

            _ => throw new NotImplementedException(),
        };
    }

    private HatchGadget BuildHatchGadget(
        HatchGadgetArchetypeData hatchGadgetArchetypeData,
        HatchGadgetInstanceData gadgetInstanceData,
        LemmingManager lemmingManager,
        TribeManager tribeManager)
    {
        throw new NotImplementedException();
    }

    public GadgetBase[] GetGadgets() => _gadgets.ToArray();
    public GadgetTrigger[] GetGadgetTriggers() => _gadgetTriggers.ToArray();
    public GadgetBehaviour[] GetGadgetBehaviours() => _gadgetBehaviours.ToArray();
}
