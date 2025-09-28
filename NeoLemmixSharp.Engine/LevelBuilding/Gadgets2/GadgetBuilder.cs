using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.Engine.LevelBuilding.Gadgets2.HatchGadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Gadgets2.HitBoxGadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Gadgets2.LogicGateGadgets;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets2;

public sealed class GadgetBuilder
{
    /// <summary>
    /// Assumption: Each gadget instance will have about this many states defined.
    /// </summary>
    private const int GadgetStateCapacityMultiplier = 3;
    /// <summary>
    /// Assumption: Each gadget state will have about this many triggers defined.
    /// </summary>
    private const int GadgetTriggerCapacityMultiplier = 4;
    /// <summary>
    /// Assumption: Each gadget state will have about this many behvaviours defined.
    /// </summary>
    private const int GadgetBehaviourCapacityMultiplier = 4;

    private readonly LevelData _levelData;
    private readonly List<GadgetBase> _gadgets;
    private readonly List<GadgetTrigger> _gadgetTriggers;
    private readonly List<GadgetBehaviour> _gadgetBehaviours;

    public GadgetBuilder(LevelData levelData)
    {
        _levelData = levelData;

        var numberOfGadgetInstances = _levelData.AllGadgetInstanceData.Count;

        // This list's required capacity is known - perfect fit
        _gadgets = new List<GadgetBase>(numberOfGadgetInstances);

        // Preallocate these lists with large(ish) initial capacities to reduce realloactions further down the line
        _gadgetTriggers = new List<GadgetTrigger>(numberOfGadgetInstances * GadgetStateCapacityMultiplier * GadgetTriggerCapacityMultiplier);
        _gadgetBehaviours = new List<GadgetBehaviour>(numberOfGadgetInstances * GadgetStateCapacityMultiplier * GadgetBehaviourCapacityMultiplier);
    }

    public void BuildLevelGadgets(LemmingManager lemmingManager, TribeManager tribeManager)
    {
        var gadgetArchetypeDataLookup = StyleCache.GetAllGadgetArchetypeData(_levelData);

        foreach (var gadgetInstanceData in _levelData.AllGadgetInstanceData)
        {
            var stylePiecePair = new StylePiecePair(gadgetInstanceData.StyleIdentifier, gadgetInstanceData.PieceIdentifier);
            var gadgetArchetypeData = gadgetArchetypeDataLookup[stylePiecePair];

            if (gadgetArchetypeData.SpecificationData.GadgetType != gadgetInstanceData.SpecificationData.GadgetType)
                throw new Exception("GadgetType mismatch!");

            var newGadget = BuildGadget(gadgetArchetypeData, gadgetInstanceData, lemmingManager, tribeManager);
            _gadgets.Add(newGadget);
        }
    }

    private GadgetBase BuildGadget(GadgetArchetypeData gadgetArchetypeData, GadgetInstanceData gadgetInstanceData, LemmingManager lemmingManager, TribeManager tribeManager)
    {
        return gadgetArchetypeData.SpecificationData.GadgetType switch
        {
            GadgetType.HitBoxGadget => BuildHitBoxGadget(gadgetArchetypeData, gadgetInstanceData, lemmingManager, tribeManager),
            GadgetType.HatchGadget => BuildHatchGadget(gadgetArchetypeData, gadgetInstanceData, tribeManager),
            GadgetType.LogicGate => BuildLogicGateGadget(gadgetArchetypeData, gadgetInstanceData),
            //  GadgetType.Counter => 
            GadgetType.LevelTimerObserver => BuildLevelTimerObserverGadget(gadgetArchetypeData, gadgetInstanceData),

            _ => throw new NotImplementedException(),
        };
    }

    private HitBoxGadget BuildHitBoxGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetInstanceData gadgetInstanceData,
        LemmingManager lemmingManager,
        TribeManager tribeManager)
    {
        var hitBoxGadgetBuilder = new HitBoxGadgetBuilder(gadgetArchetypeData, gadgetInstanceData, _gadgetTriggers, _gadgetBehaviours);

        return hitBoxGadgetBuilder.BuildHitBoxGadget(lemmingManager, tribeManager);
    }

    private HatchGadget BuildHatchGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetInstanceData gadgetInstanceData,
        TribeManager tribeManager)
    {
        var hatchGadgetBuilder = new HatchGadgetBuilder(gadgetArchetypeData, gadgetInstanceData, _gadgetTriggers, _gadgetBehaviours);

        return hatchGadgetBuilder.BuildHatchGadget(tribeManager);
    }

    private GadgetBase BuildLogicGateGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetInstanceData gadgetInstanceData)
    {
        var logicGateBuilder = new LogicGateBuilder(gadgetArchetypeData, gadgetInstanceData, _gadgetTriggers, _gadgetBehaviours);

        return logicGateBuilder.BuildLogicGateGadget();
    }

    private GadgetBase BuildLevelTimerObserverGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetInstanceData gadgetInstanceData)
    {
        throw new NotImplementedException();
    }

    public GadgetBase[] GetGadgets() => _gadgets.ToArray();
    public GadgetTrigger[] GetGadgetTriggers() => _gadgetTriggers.ToArray();
    public GadgetBehaviour[] GetGadgetBehaviours() => _gadgetBehaviours.ToArray();
}
