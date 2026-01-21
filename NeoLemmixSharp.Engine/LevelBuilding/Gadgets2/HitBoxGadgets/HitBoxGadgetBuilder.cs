using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.Data.Level.Gadget.HitBoxGadget;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets2.HitBoxGadgets;

public readonly ref struct HitBoxGadgetBuilder
{
    private readonly GadgetArchetypeData _hitBoxGadgetArchetypeData;
    private readonly HitBoxGadgetArchetypeSpecificationData _hitBoxGadgetSpecificationData;
    private readonly GadgetInstanceData _hitBoxGadgetInstanceData;
    private readonly HitBoxGadgetInstanceSpecifcationData _hitBoxGadgetInstanceSpecificationData;

    private readonly List<GadgetTrigger> _gadgetTriggers;
    private readonly List<GadgetBehaviour> _gadgetBehaviours;

    public HitBoxGadgetBuilder(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetInstanceData hitBoxGadgetData,
        List<GadgetTrigger> gadgetTriggers,
        List<GadgetBehaviour> gadgetBehaviours)
    {
        _hitBoxGadgetArchetypeData = gadgetArchetypeData;
        _hitBoxGadgetSpecificationData = (HitBoxGadgetArchetypeSpecificationData)gadgetArchetypeData.SpecificationData;
        _hitBoxGadgetInstanceData = hitBoxGadgetData;
        _hitBoxGadgetInstanceSpecificationData = (HitBoxGadgetInstanceSpecifcationData)hitBoxGadgetData.SpecificationData;

        _gadgetTriggers = gadgetTriggers;
        _gadgetBehaviours = gadgetBehaviours;
    }

    public HitBoxGadget BuildHitBoxGadget(
        LemmingManager lemmingManager,
        TribeManager tribeManager,
        ref nint dataHandleRef)
    {
        // The lemming tracker needs to be created first before any other component,
        // as it gets first dibs on the space allocated to the snapshot data.
        var lemmingTracker = new LemmingTracker(lemmingManager, ref dataHandleRef);

        var gadgetName = GadgetBuildingHelpers.GetGadgetName(_hitBoxGadgetArchetypeData, _hitBoxGadgetInstanceData);
        var gadgetBounds = GadgetBuildingHelpers.CreateHitBoxGadgetBounds(ref dataHandleRef, _hitBoxGadgetArchetypeData, _hitBoxGadgetSpecificationData, _hitBoxGadgetInstanceData, _hitBoxGadgetInstanceSpecificationData);
        var gadgetStates = BuildHitBoxGadgetStates(ref dataHandleRef, _hitBoxGadgetSpecificationData, gadgetBounds, tribeManager);
        var resizeType = GetResizeType(_hitBoxGadgetSpecificationData);

        var result = new HitBoxGadget(
            gadgetStates,
            lemmingTracker,
            _hitBoxGadgetInstanceSpecificationData.InitialStateId,
            resizeType)
        {
            DataHandle = PointerDataHelper.CreateItem<PointerWrapper>(ref dataHandleRef),
            Id = _hitBoxGadgetInstanceData.Identifier.GadgetId,
            GadgetName = gadgetName,
            CurrentGadgetBounds = gadgetBounds,

            Orientation = _hitBoxGadgetInstanceData.Orientation,
            FacingDirection = _hitBoxGadgetInstanceData.FacingDirection,
            IsFastForward = _hitBoxGadgetInstanceData.IsFastForward,
        };

        return result;
    }

    private HitBoxGadgetState[] BuildHitBoxGadgetStates(
        ref nint dataHandleRef,
        HitBoxGadgetArchetypeSpecificationData gadgetArchetypeData,
        GadgetBounds gadgetBounds,
        TribeManager tribeManager)
    {
        if (gadgetArchetypeData.GadgetStates.Length > EngineConstants.MaxAllowedNumberOfGadgetStates)
            throw new InvalidOperationException("Too many gadget states defined!");

        if (gadgetArchetypeData.GadgetStates.Length != _hitBoxGadgetInstanceSpecificationData.GadgetStates.Length)
            throw new InvalidOperationException("Mismatch in number of gadget states!");

        var result = new HitBoxGadgetState[gadgetArchetypeData.GadgetStates.Length];

        for (var i = 0; i < result.Length; i++)
        {
            var gadgetStateArchetypeData = gadgetArchetypeData.GadgetStates[i];
            var gadgetStateInstanceData = _hitBoxGadgetInstanceSpecificationData.GadgetStates[i];

            result[i] = BuildHitBoxGadgetState(
                ref dataHandleRef,
                gadgetStateArchetypeData,
                gadgetStateInstanceData,
                gadgetBounds,
                tribeManager,
                _hitBoxGadgetInstanceData.Orientation,
                _hitBoxGadgetInstanceData.FacingDirection);
        }

        return result;
    }

    private HitBoxGadgetState BuildHitBoxGadgetState(
        ref nint dataHandleRef,
        HitBoxGadgetStateArchetypeData gadgetStateArchetypeData,
        HitBoxGadgetStateInstanceData gadgetStateInstanceData,
        GadgetBounds gadgetBounds,
        TribeManager tribeManager,
        Orientation instanceOrientation,
        FacingDirection instanceFacingDirection)
    {
        var stateName = GadgetBuildingHelpers.GetGadgetStateName(gadgetStateArchetypeData, gadgetStateInstanceData);

        ReadOnlySpan<HitBoxFilterData> mainHitBoxFilterData;
        ReadOnlySpan<HitBoxFilterData> alternateHitBoxFilterData;

        if (gadgetStateArchetypeData.HitBoxFilters.Length > 0)
        {
            mainHitBoxFilterData = gadgetStateArchetypeData.HitBoxFilters;
            alternateHitBoxFilterData = gadgetStateInstanceData.CustomHitBoxFilters;
        }
        else
        {
            mainHitBoxFilterData = gadgetStateInstanceData.CustomHitBoxFilters;
            alternateHitBoxFilterData = [];
        }

        var hitBoxFilters = BuildHitBoxFilters(ref dataHandleRef, mainHitBoxFilterData, alternateHitBoxFilterData, instanceOrientation, instanceFacingDirection, tribeManager);

        var hitBoxLookup = HitBoxBuilder.BuildHitBoxLookup(
            gadgetStateArchetypeData,
            gadgetBounds);

        var generalTriggers = BuildGeneralTriggers(ref dataHandleRef, gadgetStateArchetypeData, gadgetStateInstanceData);

        return new HitBoxGadgetState(
            hitBoxFilters,
            hitBoxLookup)
        {
            StateName = stateName,
            GadgetTriggers = generalTriggers
        };
    }

    private LemmingHitBoxFilter[] BuildHitBoxFilters(
        ref nint dataHandleRef,
        ReadOnlySpan<HitBoxFilterData> mainHitBoxFilterData,
        ReadOnlySpan<HitBoxFilterData> alternateHitBoxFilterData,
        Orientation instanceOrientation,
        FacingDirection instanceFacingDirection,
        TribeManager tribeManager)
    {
        var behaviourBuilder = new GadgetBehaviourBuilder(_hitBoxGadgetInstanceData.Identifier, _gadgetBehaviours);
        var result = Helpers.GetArrayForSize<LemmingHitBoxFilter>(mainHitBoxFilterData.Length);

        var i = 0;
        foreach (var mainHitBoxFilterInstanceDatum in mainHitBoxFilterData)
        {
            var newHitBoxFilter = BuildHitBoxFilter(ref dataHandleRef, alternateHitBoxFilterData, instanceOrientation, instanceFacingDirection, tribeManager, behaviourBuilder, mainHitBoxFilterInstanceDatum);

            _gadgetTriggers.Add(newHitBoxFilter);
            result[i++] = newHitBoxFilter;
        }

        return result;
    }

    private LemmingHitBoxFilter BuildHitBoxFilter(
        ref nint dataHandleRef,
        ReadOnlySpan<HitBoxFilterData> alternateHitBoxFilterData,
        Orientation instanceOrientation,
        FacingDirection instanceFacingDirection,
        TribeManager tribeManager,
        GadgetBehaviourBuilder behaviourBuilder,
        HitBoxFilterData mainHitBoxFilterInstanceDatum)
    {
        var correspondingAlternateHitBoxFilterInstanceDatum = GetCorrespondingHitBoxFilterInstanceData(mainHitBoxFilterInstanceDatum.HitBoxFilterName, alternateHitBoxFilterData);

        var solidityType = mainHitBoxFilterInstanceDatum.SolidityType;
        var hitBoxInteractionType = mainHitBoxFilterInstanceDatum.HitBoxBehaviour;
        var gadgetTriggerName = mainHitBoxFilterInstanceDatum.HitBoxFilterName;

        ReadOnlySpan<GadgetBehaviourData> alternateLemmingHitBehaviours = correspondingAlternateHitBoxFilterInstanceDatum?.OnLemmingHitBehaviours ?? [];
        ReadOnlySpan<GadgetBehaviourData> alternateLemmingEnterBehaviours = correspondingAlternateHitBoxFilterInstanceDatum?.OnLemmingEnterBehaviours ?? [];
        ReadOnlySpan<GadgetBehaviourData> alternateLemmingPresentBehaviours = correspondingAlternateHitBoxFilterInstanceDatum?.OnLemmingPresentBehaviours ?? [];
        ReadOnlySpan<GadgetBehaviourData> alternateLemmingExitBehaviours = correspondingAlternateHitBoxFilterInstanceDatum?.OnLemmingExitBehaviours ?? [];

        var lemmingCriteriaBuilder = new LemmingCriteriaBuilder(tribeManager, instanceOrientation, instanceFacingDirection);
        var lemmingCriteria = lemmingCriteriaBuilder.BuildLemmingCriteria(mainHitBoxFilterInstanceDatum.HitBoxCriteria);
        var onLemmingHitBehaviours = behaviourBuilder.BuildBehaviours(ref dataHandleRef, mainHitBoxFilterInstanceDatum.OnLemmingHitBehaviours, alternateLemmingHitBehaviours);
        var onLemmingEnterBehaviours = behaviourBuilder.BuildBehaviours(ref dataHandleRef, mainHitBoxFilterInstanceDatum.OnLemmingEnterBehaviours, alternateLemmingEnterBehaviours);
        var onLemmingPresentBehaviours = behaviourBuilder.BuildBehaviours(ref dataHandleRef, mainHitBoxFilterInstanceDatum.OnLemmingPresentBehaviours, alternateLemmingPresentBehaviours);
        var onLemmingExitBehaviours = behaviourBuilder.BuildBehaviours(ref dataHandleRef, mainHitBoxFilterInstanceDatum.OnLemmingExitBehaviours, alternateLemmingExitBehaviours);

        var newHitBoxFilter = new LemmingHitBoxFilter(
            solidityType,
            hitBoxInteractionType,
            lemmingCriteria,
            onLemmingHitBehaviours,
            onLemmingEnterBehaviours,
            onLemmingPresentBehaviours,
            onLemmingExitBehaviours)
        {
            Id = _gadgetTriggers.Count,
            TriggerName = gadgetTriggerName,
            Behaviours = []
        };

        return newHitBoxFilter;
    }

    private static HitBoxFilterData? GetCorrespondingHitBoxFilterInstanceData(GadgetTriggerName hitBoxFilterName, ReadOnlySpan<HitBoxFilterData> hitBoxFilterData)
    {
        foreach (var hitBoxFilterDatum in hitBoxFilterData)
        {
            if (hitBoxFilterDatum.HitBoxFilterName == hitBoxFilterName)
                return hitBoxFilterDatum;
        }

        return null;
    }

    private GadgetTrigger[] BuildGeneralTriggers(ref nint dataHandleRef, IGadgetStateArchetypeData gadgetStateArchetypeData, IGadgetStateInstanceData gadgetStateInstanceData)
    {
        var gadgetTriggerBuilder = new GadgetTriggerBuilder(_hitBoxGadgetInstanceData.Identifier, _gadgetTriggers, _gadgetBehaviours);
        return gadgetTriggerBuilder.BuildGadgetTriggers(ref dataHandleRef, gadgetStateArchetypeData, gadgetStateInstanceData);
    }

    private ResizeType GetResizeType(HitBoxGadgetArchetypeSpecificationData gadgetArchetypeData)
    {
        return new DihedralTransformation(_hitBoxGadgetInstanceData.Orientation, _hitBoxGadgetInstanceData.FacingDirection).Transform(gadgetArchetypeData.ResizeType);
    }
}
