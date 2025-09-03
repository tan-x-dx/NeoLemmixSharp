using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.Data.Level.Gadget.LogicGateGadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.LogicGateGadget;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets2.LogicGateGadgets;

public readonly ref struct LogicGateBuilder
{
    private readonly GadgetIdentifier _identifier;
    private readonly List<GadgetTrigger> _gadgetTriggers;
    private readonly List<GadgetBehaviour> _gadgetBehaviours;

    public LogicGateBuilder(
        GadgetIdentifier identifier,
        List<GadgetTrigger> gadgetTriggers,
        List<GadgetBehaviour> gadgetBehaviours)
    {
        _identifier = identifier;
        _gadgetTriggers = gadgetTriggers;
        _gadgetBehaviours = gadgetBehaviours;
    }

    public GadgetBase BuildLogicGateGadget(
        LogicGateGadgetArchetypeData gadgetArchetypeData,
        LogicGateGadgetInstanceData gadgetInstanceData)
    {
        var gadgetName = GadgetBuildingHelpers.GetGadgetName(gadgetArchetypeData, gadgetInstanceData);
        var gadgetBounds = GadgetBuildingHelpers.CreateGadgetBounds(gadgetArchetypeData, gadgetInstanceData);
        var gadgetStates = BuildLogicGateStates(gadgetArchetypeData, gadgetInstanceData, gadgetBounds);

        /* return new HatchGadget(
             gadgetStates,
             gadgetInstanceData.InitialStateId,
             hatchSpawnData,
             spawnPointOffset)
         {
             Id = gadgetInstanceData.Identifier.GadgetId,
             GadgetName = gadgetName,
             CurrentGadgetBounds = gadgetBounds,

             Orientation = gadgetInstanceData.Orientation,
             FacingDirection = gadgetInstanceData.FacingDirection,
             IsFastForward = false,
         };*/

        return null;
    }

    private GadgetState[] BuildLogicGateStates(LogicGateGadgetArchetypeData gadgetArchetypeData, LogicGateGadgetInstanceData gadgetInstanceData, GadgetBounds gadgetBounds)
    {
        return gadgetArchetypeData.LogicGateGadgetType switch
        {
            LogicGateGadgetType.AndGate => BuildAndGateStates(gadgetArchetypeData, gadgetInstanceData, gadgetBounds),
            LogicGateGadgetType.OrGate => BuildOrGateStates(gadgetArchetypeData, gadgetInstanceData, gadgetBounds),
            LogicGateGadgetType.NotGate => BuildNotGateStates(gadgetArchetypeData, gadgetInstanceData, gadgetBounds),
            LogicGateGadgetType.XorGate => BuildXorGateStates(gadgetArchetypeData, gadgetInstanceData, gadgetBounds),

            _ => Helpers.ThrowUnknownEnumValueException<LogicGateGadgetType, GadgetState[]>(gadgetArchetypeData.LogicGateGadgetType),
        };
    }

    private GadgetState[] BuildAndGateStates(LogicGateGadgetArchetypeData gadgetArchetypeData, LogicGateGadgetInstanceData gadgetInstanceData, GadgetBounds gadgetBounds)
    {
        throw new NotImplementedException();
    }

    private GadgetState[] BuildOrGateStates(LogicGateGadgetArchetypeData gadgetArchetypeData, LogicGateGadgetInstanceData gadgetInstanceData, GadgetBounds gadgetBounds)
    {
        throw new NotImplementedException();
    }

    private GadgetState[] BuildNotGateStates(LogicGateGadgetArchetypeData gadgetArchetypeData, LogicGateGadgetInstanceData gadgetInstanceData, GadgetBounds gadgetBounds)
    {
        throw new NotImplementedException();
    }

    private GadgetState[] BuildXorGateStates(LogicGateGadgetArchetypeData gadgetArchetypeData, LogicGateGadgetInstanceData gadgetInstanceData, GadgetBounds gadgetBounds)
    {
        throw new NotImplementedException();
    }
}
