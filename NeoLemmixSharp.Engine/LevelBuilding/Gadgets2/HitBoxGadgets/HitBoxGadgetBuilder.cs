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
    private readonly GadgetIdentifier _gadgetIdentifier;
    private readonly List<GadgetTrigger> _gadgetTriggers;
    private readonly List<GadgetBehaviour> _gadgetBehaviours;

    public HitBoxGadgetBuilder(
        GadgetIdentifier gadgetIdentifier,
        List<GadgetTrigger> gadgetTriggers,
        List<GadgetBehaviour> gadgetBehaviours)
    {
        _gadgetIdentifier = gadgetIdentifier;
        _gadgetTriggers = gadgetTriggers;
        _gadgetBehaviours = gadgetBehaviours;
    }

    public HitBoxGadget BuildHitBoxGadget(
        HitBoxGadgetArchetypeData gadgetArchetypeData,
        HitBoxGadgetInstanceData gadgetInstanceData,
        LemmingManager lemmingManager,
        TribeManager tribeManager)
    {
        var lemmingTracker = new LemmingTracker(lemmingManager);

        var gadgetName = GadgetBuildingHelpers.GetGadgetName(gadgetArchetypeData, gadgetInstanceData);
        var gadgetBounds = GadgetBuildingHelpers.CreateHitBoxGadgetBounds(gadgetArchetypeData, gadgetInstanceData);
        var gadgetStates = BuildHitBoxGadgetStates(gadgetArchetypeData, gadgetInstanceData, gadgetBounds, tribeManager);
        var resizeType = GetResizeType(gadgetArchetypeData, gadgetInstanceData);

        return new HitBoxGadget(
            gadgetStates,
            gadgetInstanceData.InitialStateId,
            resizeType,
            lemmingTracker)
        {
            Id = gadgetInstanceData.Identifier.GadgetId,
            GadgetName = gadgetName,
            CurrentGadgetBounds = gadgetBounds,

            Orientation = gadgetInstanceData.Orientation,
            FacingDirection = gadgetInstanceData.FacingDirection,
            IsFastForward = gadgetInstanceData.IsFastForward,
        };
    }

    private HitBoxGadgetState[] BuildHitBoxGadgetStates(
        HitBoxGadgetArchetypeData gadgetArchetypeData,
        HitBoxGadgetInstanceData gadgetInstanceData,
        GadgetBounds gadgetBounds,
        TribeManager tribeManager)
    {
        if (gadgetArchetypeData.GadgetStates.Length > EngineConstants.MaxAllowedNumberOfGadgetStates)
            throw new InvalidOperationException("Too many gadget states defined!");

        if (gadgetArchetypeData.GadgetStates.Length != gadgetInstanceData.GadgetStates.Length)
            throw new InvalidOperationException("Mismatch in number of gadget states!");

        var result = new HitBoxGadgetState[gadgetArchetypeData.GadgetStates.Length];

        for (var i = 0; i < result.Length; i++)
        {
            var gadgetStateArchetypeData = gadgetArchetypeData.GadgetStates[i];
            var gadgetStateInstanceData = gadgetInstanceData.GadgetStates[i];

            result[i] = BuildHitBoxGadgetState(
                gadgetStateArchetypeData,
                gadgetStateInstanceData,
                gadgetBounds,
                tribeManager,
                gadgetInstanceData.Orientation,
                gadgetInstanceData.FacingDirection);
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
        var behaviourBuilder = new GadgetBehaviourBuilder(_gadgetIdentifier, _gadgetBehaviours);
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
                GadgetBehaviours = []
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
        var gadgetTriggerBuilder = new GadgetTriggerBuilder(_gadgetIdentifier, _gadgetTriggers, _gadgetBehaviours);
        return gadgetTriggerBuilder.BuildGadgetTriggers(gadgetStateArchetypeData, gadgetStateInstanceData);
    }

    private static ResizeType GetResizeType(HitBoxGadgetArchetypeData gadgetArchetypeData, HitBoxGadgetInstanceData gadgetInstanceData)
    {
        return new DihedralTransformation(gadgetInstanceData.Orientation, gadgetInstanceData.FacingDirection).Transform(gadgetArchetypeData.ResizeType);
    }
}
