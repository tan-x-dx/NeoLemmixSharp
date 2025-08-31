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
        throw new NotImplementedException();
    }
}
