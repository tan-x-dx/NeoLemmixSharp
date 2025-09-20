using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.Data.Level.Gadget.LogicGateGadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.LogicGateGadget;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets2.LogicGateGadgets;

public readonly ref struct LogicGateBuilder
{
    private readonly GadgetInstanceData _logicGateGadgetInstanceData;
    private readonly LogicGateGadgetTypeInstanceData _logicGateGadgetTypeInstanceData;
    private readonly List<GadgetTrigger> _gadgetTriggers;
    private readonly List<GadgetBehaviour> _gadgetBehaviours;

    public LogicGateBuilder(
        GadgetInstanceData logicGateGadgetData,
        List<GadgetTrigger> gadgetTriggers,
        List<GadgetBehaviour> gadgetBehaviours)
    {
        _logicGateGadgetInstanceData = logicGateGadgetData;
        _logicGateGadgetTypeInstanceData = (LogicGateGadgetTypeInstanceData)logicGateGadgetData.GadgetTypeInstanceData;
        _gadgetTriggers = gadgetTriggers;
        _gadgetBehaviours = gadgetBehaviours;
    }

    public GadgetBase BuildLogicGateGadget(LogicGateGadgetArchetypeData gadgetArchetypeData)
    {
        var gadgetName = GadgetBuildingHelpers.GetGadgetName(gadgetArchetypeData, _logicGateGadgetInstanceData);
        var gadgetBounds = GadgetBuildingHelpers.CreateGadgetBounds(gadgetArchetypeData, _logicGateGadgetInstanceData);
        var gadgetStates = BuildLogicGateStates(gadgetArchetypeData, gadgetBounds);

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

    private GadgetState[] BuildLogicGateStates(
        LogicGateGadgetArchetypeData gadgetArchetypeData,
        GadgetBounds gadgetBounds)
    {
        return gadgetArchetypeData.LogicGateGadgetType switch
        {
            LogicGateGadgetType.AndGate => BuildAndGateStates(gadgetArchetypeData, gadgetBounds),
            LogicGateGadgetType.OrGate => BuildOrGateStates(gadgetArchetypeData, gadgetBounds),
            LogicGateGadgetType.NotGate => BuildNotGateStates(gadgetArchetypeData, gadgetBounds),
            LogicGateGadgetType.XorGate => BuildXorGateStates(gadgetArchetypeData, gadgetBounds),

            _ => Helpers.ThrowUnknownEnumValueException<LogicGateGadgetType, GadgetState[]>(gadgetArchetypeData.LogicGateGadgetType),
        };
    }

    private GadgetState[] BuildAndGateStates(LogicGateGadgetArchetypeData gadgetArchetypeData, GadgetBounds gadgetBounds)
    {
        throw new NotImplementedException();
    }

    private GadgetState[] BuildOrGateStates(LogicGateGadgetArchetypeData gadgetArchetypeData, GadgetBounds gadgetBounds)
    {
        throw new NotImplementedException();
    }

    private GadgetState[] BuildNotGateStates(LogicGateGadgetArchetypeData gadgetArchetypeData, GadgetBounds gadgetBounds)
    {
        throw new NotImplementedException();
    }

    private GadgetState[] BuildXorGateStates(LogicGateGadgetArchetypeData gadgetArchetypeData, GadgetBounds gadgetBounds)
    {
        throw new NotImplementedException();
    }
}
