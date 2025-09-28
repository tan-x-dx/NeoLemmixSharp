using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.Data.Level.Gadget.LogicGateGadget;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.LogicGateGadget;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets2.LogicGateGadgets;

public readonly ref struct LogicGateBuilder
{
    private readonly GadgetArchetypeData _logicGateArchetypeData;
    private readonly LogicGateGadgetArchetypeSpecificationData _logicGateArchetypeSpecificationData;
    private readonly GadgetInstanceData _logicGateGadgetInstanceData;
    private readonly LogicGateGadgetInstanceSpecificationData _logicGateGadgetInstanceSpecificationData;

    private readonly List<GadgetTrigger> _gadgetTriggers;
    private readonly List<GadgetBehaviour> _gadgetBehaviours;

    public LogicGateBuilder(
        GadgetArchetypeData logicGateArchetypeData,
        GadgetInstanceData logicGateGadgetData,
        List<GadgetTrigger> gadgetTriggers,
        List<GadgetBehaviour> gadgetBehaviours)
    {
        _logicGateArchetypeData = logicGateArchetypeData;
        _logicGateArchetypeSpecificationData = (LogicGateGadgetArchetypeSpecificationData)logicGateArchetypeData.SpecificationData;
        _logicGateGadgetInstanceData = logicGateGadgetData;
        _logicGateGadgetInstanceSpecificationData = (LogicGateGadgetInstanceSpecificationData)logicGateGadgetData.SpecificationData;

        _gadgetTriggers = gadgetTriggers;
        _gadgetBehaviours = gadgetBehaviours;
    }

    public GadgetBase BuildLogicGateGadget()
    {
        var gadgetName = GadgetBuildingHelpers.GetGadgetName(_logicGateArchetypeData, _logicGateGadgetInstanceData);
        var gadgetBounds = GadgetBuildingHelpers.CreateGadgetBounds(_logicGateArchetypeData, _logicGateGadgetInstanceData);
        var gadgetStates = BuildLogicGateStates(gadgetBounds);

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

    private GadgetState[] BuildLogicGateStates(GadgetBounds gadgetBounds)
    {
        return _logicGateGadgetInstanceSpecificationData.LogicGateGadgetType switch
        {
            LogicGateGadgetType.AndGate => BuildAndGateStates(gadgetBounds),
            LogicGateGadgetType.OrGate => BuildOrGateStates(gadgetBounds),
            LogicGateGadgetType.NotGate => BuildNotGateStates(gadgetBounds),
            LogicGateGadgetType.XorGate => BuildXorGateStates(gadgetBounds),

            _ => Helpers.ThrowUnknownEnumValueException<LogicGateGadgetType, GadgetState[]>(_logicGateGadgetInstanceSpecificationData.LogicGateGadgetType),
        };
    }

    private GadgetState[] BuildAndGateStates(GadgetBounds gadgetBounds)
    {
        throw new NotImplementedException();
    }

    private GadgetState[] BuildOrGateStates(GadgetBounds gadgetBounds)
    {
        throw new NotImplementedException();
    }

    private GadgetState[] BuildNotGateStates(GadgetBounds gadgetBounds)
    {
        throw new NotImplementedException();
    }

    private GadgetState[] BuildXorGateStates(GadgetBounds gadgetBounds)
    {
        throw new NotImplementedException();
    }
}
