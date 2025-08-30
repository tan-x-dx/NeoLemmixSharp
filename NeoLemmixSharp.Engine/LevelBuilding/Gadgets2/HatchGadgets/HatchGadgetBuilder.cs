using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Level.Gadgets.Hatch;
using NeoLemmixSharp.IO.Data.Style.Gadget.Hatch;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets2.HatchGadgets;

public readonly struct HatchGadgetBuilder
{
    private readonly GadgetIdentifier _gadgetIdentifier;
    private readonly List<GadgetTrigger> _gadgetTriggers;
    private readonly List<GadgetBehaviour> _gadgetBehaviours;

    public HatchGadgetBuilder(
        GadgetIdentifier gadgetIdentifier,
        List<GadgetTrigger> gadgetTriggers,
        List<GadgetBehaviour> gadgetBehaviours)
    {
        _gadgetIdentifier = gadgetIdentifier;
        _gadgetTriggers = gadgetTriggers;
        _gadgetBehaviours = gadgetBehaviours;
    }

    public HatchGadget BuildHatchGadget(
        HatchGadgetArchetypeData gadgetArchetypeData,
        HatchGadgetInstanceData gadgetInstanceData,
        LemmingManager lemmingManager,
        TribeManager tribeManager)
    {
        var gadgetName = GadgetBuildingHelpers.GetGadgetName(gadgetArchetypeData, gadgetInstanceData);
        var gadgetBounds = GadgetBuildingHelpers.CreateGadgetBounds(gadgetArchetypeData, gadgetInstanceData);
        var gadgetStates = BuildHatchGadgetStates(gadgetArchetypeData, gadgetInstanceData, gadgetBounds, tribeManager);
        var hatchSpawnData = BuildHatchSpawnData(gadgetInstanceData, tribeManager);
        var spawnPointOffset = BuildSpawnPointOffset(gadgetArchetypeData, gadgetInstanceData);

        return new HatchGadget(
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
        };
    }

    private HatchGadgetState[] BuildHatchGadgetStates(
        HatchGadgetArchetypeData gadgetArchetypeData,
        HatchGadgetInstanceData gadgetInstanceData,
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
            var gadgetStateInstanceData = gadgetInstanceData.GadgetStates[i];

            result[i] = BuildHatchGadgetState(
                gadgetStateArchetypeData,
                gadgetStateInstanceData,
                gadgetBounds,
                tribeManager,
                gadgetInstanceData.Orientation,
                gadgetInstanceData.FacingDirection);
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
        var gadgetTriggerBuilder = new GadgetTriggerBuilder(_gadgetIdentifier, _gadgetTriggers, _gadgetBehaviours);
        return gadgetTriggerBuilder.BuildGadgetTriggers(gadgetStateArchetypeData, gadgetStateInstanceData);
    }

    private static HatchSpawnData BuildHatchSpawnData(
        HatchGadgetInstanceData gadgetInstanceData,
        TribeManager tribeManager)
    {
        return new HatchSpawnData(
            gadgetInstanceData.HatchGroupId,
            tribeManager.AllItems[gadgetInstanceData.TribeId],
            gadgetInstanceData.RawStateData,
            gadgetInstanceData.Orientation,
            gadgetInstanceData.FacingDirection,
            gadgetInstanceData.NumberOfLemmingsToRelease);
    }

    private static Point BuildSpawnPointOffset(HatchGadgetArchetypeData gadgetArchetypeData, HatchGadgetInstanceData gadgetInstanceData)
    {
        var spawnPointOffset = gadgetArchetypeData.SpawnOffset;
        var gadgetSize = gadgetArchetypeData.BaseSpriteSize;

        var dht = new DihedralTransformation(gadgetInstanceData.Orientation, gadgetInstanceData.FacingDirection);
        return dht.Transform(spawnPointOffset, gadgetSize);
    }
}
