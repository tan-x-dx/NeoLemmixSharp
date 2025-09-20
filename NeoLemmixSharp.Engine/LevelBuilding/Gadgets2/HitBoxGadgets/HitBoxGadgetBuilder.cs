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
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadgetGadget;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets2.HitBoxGadgets;

public readonly ref struct HitBoxGadgetBuilder
{
    private readonly GadgetInstanceData _hitBoxGadgetInstanceData;
    private readonly HitBoxGadgetTypeInstanceData _hitBoxGadgetTypeInstanceData;
    private readonly List<GadgetTrigger> _gadgetTriggers;
    private readonly List<GadgetBehaviour> _gadgetBehaviours;

    public HitBoxGadgetBuilder(
        GadgetInstanceData hitBoxGadgetData,
        List<GadgetTrigger> gadgetTriggers,
        List<GadgetBehaviour> gadgetBehaviours)
    {
        _hitBoxGadgetInstanceData = hitBoxGadgetData;
        _hitBoxGadgetTypeInstanceData = (HitBoxGadgetTypeInstanceData)hitBoxGadgetData.GadgetTypeInstanceData;
        _gadgetTriggers = gadgetTriggers;
        _gadgetBehaviours = gadgetBehaviours;
    }

    public HitBoxGadget BuildHitBoxGadget(
        HitBoxGadgetArchetypeData gadgetArchetypeData,
        LemmingManager lemmingManager,
        TribeManager tribeManager)
    {
        var lemmingTracker = new LemmingTracker(lemmingManager);

        var gadgetName = GadgetBuildingHelpers.GetGadgetName(gadgetArchetypeData, _hitBoxGadgetInstanceData);
        var gadgetBounds = GadgetBuildingHelpers.CreateHitBoxGadgetBounds(gadgetArchetypeData, _hitBoxGadgetInstanceData, _hitBoxGadgetTypeInstanceData);
        var gadgetStates = BuildHitBoxGadgetStates(gadgetArchetypeData, gadgetBounds, tribeManager);
        var resizeType = GetResizeType(gadgetArchetypeData);

        return new HitBoxGadget(
            gadgetStates,
            _hitBoxGadgetTypeInstanceData.InitialStateId,
            resizeType,
            lemmingTracker)
        {
            Id = _hitBoxGadgetInstanceData.Identifier.GadgetId,
            GadgetName = gadgetName,
            CurrentGadgetBounds = gadgetBounds,

            Orientation = _hitBoxGadgetInstanceData.Orientation,
            FacingDirection = _hitBoxGadgetInstanceData.FacingDirection,
            IsFastForward = _hitBoxGadgetInstanceData.IsFastForward,
        };
    }

    private HitBoxGadgetState[] BuildHitBoxGadgetStates(
        HitBoxGadgetArchetypeData gadgetArchetypeData,
        GadgetBounds gadgetBounds,
        TribeManager tribeManager)
    {
        if (gadgetArchetypeData.GadgetStates.Length > EngineConstants.MaxAllowedNumberOfGadgetStates)
            throw new InvalidOperationException("Too many gadget states defined!");

        if (gadgetArchetypeData.GadgetStates.Length != _hitBoxGadgetTypeInstanceData.GadgetStates.Length)
            throw new InvalidOperationException("Mismatch in number of gadget states!");

        var result = new HitBoxGadgetState[gadgetArchetypeData.GadgetStates.Length];

        for (var i = 0; i < result.Length; i++)
        {
            var gadgetStateArchetypeData = gadgetArchetypeData.GadgetStates[i];
            var gadgetStateInstanceData = _hitBoxGadgetTypeInstanceData.GadgetStates[i];

            result[i] = BuildHitBoxGadgetState(
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

        var hitBoxFilters = BuildHitBoxFilters(mainHitBoxFilterData, alternateHitBoxFilterData, instanceOrientation, instanceFacingDirection, tribeManager);

        var hitBoxLookup = HitBoxBuilder.BuildHitBoxLookup(
            gadgetStateArchetypeData,
            gadgetBounds);

        var generalTriggers = BuildGeneralTriggers(gadgetStateArchetypeData, gadgetStateInstanceData);

        return new HitBoxGadgetState(
            hitBoxFilters,
            hitBoxLookup)
        {
            StateName = stateName,
            GadgetTriggers = generalTriggers
        };
    }

    private LemmingHitBoxFilter[] BuildHitBoxFilters(
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
            var correspondingAlternateHitBoxFilterInstanceDatum = GetCorrespondingHitBoxFilterInstanceData(mainHitBoxFilterInstanceDatum.HitBoxFilterName, alternateHitBoxFilterData);

            var solidityType = mainHitBoxFilterInstanceDatum.SolidityType;
            var hitBoxInteractionType = mainHitBoxFilterInstanceDatum.HitBoxBehaviour;
            var gadgetTriggerName = mainHitBoxFilterInstanceDatum.HitBoxFilterName.ToTriggerName();

            ReadOnlySpan<GadgetBehaviourData> alternateLemmingHitBehaviours = correspondingAlternateHitBoxFilterInstanceDatum?.OnLemmingHitBehaviours ?? [];
            ReadOnlySpan<GadgetBehaviourData> alternateLemmingEnterBehaviours = correspondingAlternateHitBoxFilterInstanceDatum?.OnLemmingEnterBehaviours ?? [];
            ReadOnlySpan<GadgetBehaviourData> alternateLemmingPresentBehaviours = correspondingAlternateHitBoxFilterInstanceDatum?.OnLemmingPresentBehaviours ?? [];
            ReadOnlySpan<GadgetBehaviourData> alternateLemmingExitBehaviours = correspondingAlternateHitBoxFilterInstanceDatum?.OnLemmingExitBehaviours ?? [];

            var lemmingCriteriaBuilder = new LemmingCriteriaBuilder(tribeManager, instanceOrientation, instanceFacingDirection);
            var lemmingCriteria = lemmingCriteriaBuilder.BuildLemmingCriteria(mainHitBoxFilterInstanceDatum.HitBoxCriteria);
            var onLemmingHitBehaviours = behaviourBuilder.BuildBehaviours(mainHitBoxFilterInstanceDatum.OnLemmingHitBehaviours, alternateLemmingHitBehaviours);
            var onLemmingEnterBehaviours = behaviourBuilder.BuildBehaviours(mainHitBoxFilterInstanceDatum.OnLemmingEnterBehaviours, alternateLemmingEnterBehaviours);
            var onLemmingPresentBehaviours = behaviourBuilder.BuildBehaviours(mainHitBoxFilterInstanceDatum.OnLemmingPresentBehaviours, alternateLemmingPresentBehaviours);
            var onLemmingExitBehaviours = behaviourBuilder.BuildBehaviours(mainHitBoxFilterInstanceDatum.OnLemmingExitBehaviours, alternateLemmingExitBehaviours);

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

            _gadgetTriggers.Add(newHitBoxFilter);

            result[i++] = newHitBoxFilter;
        }

        return result;
    }

    private static HitBoxFilterData? GetCorrespondingHitBoxFilterInstanceData(HitBoxFilterName hitBoxFilterName, ReadOnlySpan<HitBoxFilterData> hitBoxFilterData)
    {
        foreach (var hitBoxFilterDatum in hitBoxFilterData)
        {
            if (hitBoxFilterDatum.HitBoxFilterName == hitBoxFilterName)
                return hitBoxFilterDatum;
        }

        return null;
    }

    private GadgetTrigger[] BuildGeneralTriggers(IGadgetStateArchetypeData gadgetStateArchetypeData, IGadgetStateInstanceData gadgetStateInstanceData)
    {
        var gadgetTriggerBuilder = new GadgetTriggerBuilder(_hitBoxGadgetInstanceData.Identifier, _gadgetTriggers, _gadgetBehaviours);
        return gadgetTriggerBuilder.BuildGadgetTriggers(gadgetStateArchetypeData, gadgetStateInstanceData);
    }

    private ResizeType GetResizeType(HitBoxGadgetArchetypeData gadgetArchetypeData)
    {
        return new DihedralTransformation(_hitBoxGadgetInstanceData.Orientation, _hitBoxGadgetInstanceData.FacingDirection).Transform(gadgetArchetypeData.ResizeType);
    }
}
