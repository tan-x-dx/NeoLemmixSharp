using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.Data.Level.Gadget.HatchGadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.HatchGadget;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets2.HatchGadgets;

public readonly struct HatchGadgetBuilder
{
    private readonly GadgetInstanceData _hatchGadgetInstanceData;
    private readonly HatchGadgetTypeInstanceData _hatchGadgetTypeInstanceData;
    private readonly List<GadgetTrigger> _gadgetTriggers;
    private readonly List<GadgetBehaviour> _gadgetBehaviours;

    public HatchGadgetBuilder(
        GadgetInstanceData hatchGadgetData,
        List<GadgetTrigger> gadgetTriggers,
        List<GadgetBehaviour> gadgetBehaviours)
    {
        _hatchGadgetInstanceData = hatchGadgetData;
        _hatchGadgetTypeInstanceData = (HatchGadgetTypeInstanceData)hatchGadgetData.GadgetTypeInstanceData;
        _gadgetTriggers = gadgetTriggers;
        _gadgetBehaviours = gadgetBehaviours;
    }

    public HatchGadget BuildHatchGadget(
        HatchGadgetArchetypeData gadgetArchetypeData,
        TribeManager tribeManager)
    {
        var gadgetName = GadgetBuildingHelpers.GetGadgetName(gadgetArchetypeData, _hatchGadgetInstanceData);
        var gadgetBounds = GadgetBuildingHelpers.CreateGadgetBounds(gadgetArchetypeData, _hatchGadgetInstanceData);
        var gadgetStates = BuildHatchGadgetStates(gadgetArchetypeData, gadgetBounds, tribeManager);
        var hatchSpawnData = BuildHatchSpawnData(tribeManager);
        var spawnPointOffset = BuildSpawnPointOffset(gadgetArchetypeData);

        return new HatchGadget(
            gadgetStates,
            _hatchGadgetTypeInstanceData.InitialStateId,
            hatchSpawnData,
            spawnPointOffset)
        {
            Id = _hatchGadgetInstanceData.Identifier.GadgetId,
            GadgetName = gadgetName,
            CurrentGadgetBounds = gadgetBounds,

            Orientation = _hatchGadgetInstanceData.Orientation,
            FacingDirection = _hatchGadgetInstanceData.FacingDirection,
            IsFastForward = false,
        };
    }

    private HatchGadgetState[] BuildHatchGadgetStates(
        HatchGadgetArchetypeData gadgetArchetypeData,
        GadgetBounds gadgetBounds,
        TribeManager tribeManager)
    {
        if (gadgetArchetypeData.GadgetStates.Length != EngineConstants.ExpectedNumberOfHatchGadgetStates)
            throw new InvalidOperationException("Invalid amount of hatch gadget states defined!");

        if (gadgetArchetypeData.GadgetStates.Length != EngineConstants.ExpectedNumberOfHatchGadgetStates)
            throw new InvalidOperationException("Invalid amount of hatch gadget states defined!");

        var result = new HatchGadgetState[EngineConstants.ExpectedNumberOfHatchGadgetStates];

        for (var i = 0; i < result.Length; i++)
        {
            var gadgetStateArchetypeData = gadgetArchetypeData.GadgetStates[i];
            var gadgetStateInstanceData = _hatchGadgetTypeInstanceData.GadgetStates[i];

            result[i] = BuildHatchGadgetState(
                gadgetStateArchetypeData,
                gadgetStateInstanceData,
                gadgetBounds,
                tribeManager,
                _hatchGadgetInstanceData.Orientation,
                _hatchGadgetInstanceData.FacingDirection);
        }

        return result;
    }

    private HatchGadgetState BuildHatchGadgetState(
        HatchGadgetStateArchetypeData gadgetStateArchetypeData,
        HatchGadgetStateInstanceData gadgetStateInstanceData,
        GadgetBounds gadgetBounds,
        TribeManager tribeManager,
        Orientation orientation,
        FacingDirection facingDirection)
    {
        throw new NotImplementedException();

        var stateName = GadgetBuildingHelpers.GetGadgetStateName(gadgetStateArchetypeData, gadgetStateInstanceData);

        var triggers = BuildTriggers(gadgetStateArchetypeData, gadgetStateInstanceData);

        return new HatchGadgetState()
        {
            StateName = stateName,
            GadgetTriggers = triggers
        };
    }

    private GadgetTrigger[] BuildTriggers(HatchGadgetStateArchetypeData gadgetStateArchetypeData, HatchGadgetStateInstanceData gadgetStateInstanceData)
    {
        var gadgetTriggerBuilder = new GadgetTriggerBuilder(_hatchGadgetInstanceData.Identifier, _gadgetTriggers, _gadgetBehaviours);
        return gadgetTriggerBuilder.BuildGadgetTriggers(gadgetStateArchetypeData, gadgetStateInstanceData);
    }

    private HatchSpawnData BuildHatchSpawnData(
        TribeManager tribeManager)
    {
        return new HatchSpawnData(
            _hatchGadgetTypeInstanceData.HatchGroupId,
            tribeManager.AllItems[_hatchGadgetTypeInstanceData.TribeId],
            _hatchGadgetTypeInstanceData.RawStateData,
            _hatchGadgetInstanceData.Orientation,
            _hatchGadgetInstanceData.FacingDirection,
            _hatchGadgetTypeInstanceData.NumberOfLemmingsToRelease);
    }

    private Point BuildSpawnPointOffset(HatchGadgetArchetypeData gadgetArchetypeData)
    {
        var spawnPointOffset = gadgetArchetypeData.SpawnOffset;
        var gadgetSize = gadgetArchetypeData.BaseSpriteSize;

        var dht = new DihedralTransformation(_hatchGadgetInstanceData.Orientation, _hatchGadgetInstanceData.FacingDirection);
        return dht.Transform(spawnPointOffset, gadgetSize);
    }
}
