using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.Data.Level.Gadget.HatchGadget;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.HatchGadget;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets2.HatchGadgets;

public readonly ref struct HatchGadgetBuilder
{
    private readonly GadgetArchetypeData _hatchGadgetArchetypeData;
    private readonly HatchGadgetArchetypeSpecificationData _hatchGadgetSpecificationData;
    private readonly GadgetInstanceData _hatchGadgetInstanceData;
    private readonly HatchGadgetInstanceSpecificationData _hatchGadgetTypeInstanceData;

    private readonly List<GadgetTrigger> _gadgetTriggers;
    private readonly List<GadgetBehaviour> _gadgetBehaviours;

    public HatchGadgetBuilder(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetInstanceData gadgetInstanceData,
        List<GadgetTrigger> gadgetTriggers,
        List<GadgetBehaviour> gadgetBehaviours)
    {
        _hatchGadgetArchetypeData = gadgetArchetypeData;
        _hatchGadgetSpecificationData = (HatchGadgetArchetypeSpecificationData)gadgetArchetypeData.SpecificationData;
        _hatchGadgetInstanceData = gadgetInstanceData;
        _hatchGadgetTypeInstanceData = (HatchGadgetInstanceSpecificationData)gadgetInstanceData.SpecificationData;

        _gadgetTriggers = gadgetTriggers;
        _gadgetBehaviours = gadgetBehaviours;
    }

    public HatchGadget BuildHatchGadget(TribeManager tribeManager, ref nint dataHandleRef)
    {
        var gadgetName = GadgetBuildingHelpers.GetGadgetName(_hatchGadgetArchetypeData, _hatchGadgetInstanceData);
        var gadgetBounds = GadgetBuildingHelpers.CreateGadgetBounds(ref dataHandleRef, _hatchGadgetArchetypeData, _hatchGadgetInstanceData);
        var gadgetStates = BuildHatchGadgetStates(ref dataHandleRef, gadgetBounds, tribeManager);
        var hatchSpawnData = BuildHatchSpawnData(ref dataHandleRef);
        var spawnPointOffset = BuildSpawnPointOffset();

        var result = new HatchGadget(
            gadgetStates,
            _hatchGadgetTypeInstanceData.InitialStateId,
            hatchSpawnData,
            spawnPointOffset)
        {
            DataHandle = PointerDataHelper.CreateItem<PointerWrapper>(ref dataHandleRef),
            Id = _hatchGadgetInstanceData.Identifier.GadgetId,
            GadgetName = gadgetName,
            CurrentGadgetBounds = gadgetBounds,

            Orientation = _hatchGadgetInstanceData.Orientation,
            FacingDirection = _hatchGadgetInstanceData.FacingDirection,
            IsFastForward = false,
        };

        return result;
    }

    private HatchGadgetState[] BuildHatchGadgetStates(
        ref nint dataHandleRef,
        GadgetBounds gadgetBounds,
        TribeManager tribeManager)
    {
        if (_hatchGadgetSpecificationData.GadgetStates.Length != EngineConstants.ExpectedNumberOfHatchGadgetStates)
            throw new InvalidOperationException("Invalid amount of hatch gadget states defined!");

        if (_hatchGadgetTypeInstanceData.GadgetStates.Length != EngineConstants.ExpectedNumberOfHatchGadgetStates)
            throw new InvalidOperationException("Invalid amount of hatch gadget states defined!");

        var result = new HatchGadgetState[EngineConstants.ExpectedNumberOfHatchGadgetStates];

        for (var i = 0; i < result.Length; i++)
        {
            var gadgetStateArchetypeData = _hatchGadgetSpecificationData.GadgetStates[i];
            var gadgetStateInstanceData = _hatchGadgetTypeInstanceData.GadgetStates[i];

            result[i] = BuildHatchGadgetState(
                ref dataHandleRef,
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
        ref nint dataHandleRef,
        HatchGadgetStateArchetypeData gadgetStateArchetypeData,
        HatchGadgetStateInstanceData gadgetStateInstanceData,
        GadgetBounds gadgetBounds,
        TribeManager tribeManager,
        Orientation orientation,
        FacingDirection facingDirection)
    {
        var stateName = GadgetBuildingHelpers.GetGadgetStateName(gadgetStateArchetypeData, gadgetStateInstanceData);

        var triggers = BuildTriggers(ref dataHandleRef, gadgetStateArchetypeData, gadgetStateInstanceData);

        return new HatchGadgetState()
        {
            StateName = stateName,
            GadgetTriggers = triggers,
            Type = gadgetStateArchetypeData.Type,
        };
    }

    private GadgetTrigger[] BuildTriggers(
        ref nint dataHandleRef,
        HatchGadgetStateArchetypeData gadgetStateArchetypeData,
        HatchGadgetStateInstanceData gadgetStateInstanceData)
    {
        var gadgetTriggerBuilder = new GadgetTriggerBuilder(_hatchGadgetInstanceData.Identifier, _gadgetTriggers, _gadgetBehaviours);
        return gadgetTriggerBuilder.BuildGadgetTriggers(ref dataHandleRef, gadgetStateArchetypeData, gadgetStateInstanceData);
    }

    private HatchSpawnData BuildHatchSpawnData(ref nint dataHandleRef)
    {
        var result = new HatchSpawnData(
            ref dataHandleRef,
            _hatchGadgetTypeInstanceData.NumberOfLemmingsToRelease,
            _hatchGadgetInstanceData.Orientation,
            _hatchGadgetInstanceData.FacingDirection,
            _hatchGadgetTypeInstanceData.TribeId,
            _hatchGadgetTypeInstanceData.RawStateData,
            _hatchGadgetTypeInstanceData.HatchGroupId);

        return result;
    }

    private Point BuildSpawnPointOffset()
    {
        var spawnPointOffset = _hatchGadgetSpecificationData.SpawnOffset;
        var gadgetSize = _hatchGadgetArchetypeData.BaseSpriteSize;

        var dht = new DihedralTransformation(_hatchGadgetInstanceData.Orientation, _hatchGadgetInstanceData.FacingDirection);
        return dht.Transform(spawnPointOffset, gadgetSize);
    }
}
