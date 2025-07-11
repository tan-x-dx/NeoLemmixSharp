using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public static class HitBoxGadgetArchetypeBuilder
{
    public static HitBoxGadget BuildGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetData gadgetData,
        LemmingManager lemmingManager,
        TribeManager tribeManager)
    {
        var gadgetName = GadgetBuildingHelpers.GetGadgetName(gadgetArchetypeData, gadgetData);
        var gadgetBounds = GadgetBuildingHelpers.CreateGadgetBounds(gadgetArchetypeData, gadgetData);
        var gadgetStates = BuildGadgetStates(gadgetArchetypeData, gadgetData, gadgetBounds, tribeManager);
        var resizeType = GetResizeType(gadgetArchetypeData, gadgetData);
        var lemmingTracker = new LemmingTracker(lemmingManager);

        return new HitBoxGadget(
            gadgetName,
            gadgetStates,
            gadgetData.InitialStateId,
            resizeType,
            lemmingTracker)
        {
            Id = gadgetData.Identifier.GadgetId,
            Orientation = gadgetData.Orientation,
            FacingDirection = gadgetData.FacingDirection,

            CurrentGadgetBounds = gadgetBounds,

            IsFastForward = true
        };
    }

    private static GadgetState[] BuildGadgetStates(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetData gadgetData,
        GadgetBounds gadgetBounds,
        TribeManager tribeManager)
    {
        Debug.Assert(gadgetArchetypeData.AllGadgetStateData.Length == EngineConstants.NumberOfAllowedStatesForFunctionalGadgets);

        var result = new GadgetState[gadgetArchetypeData.AllGadgetStateData.Length];

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = BuildGadgetState(gadgetArchetypeData, gadgetData, gadgetBounds, i, gadgetArchetypeData.BaseSpriteSize, tribeManager);
        }

        return result;
    }

    private static GadgetState BuildGadgetState(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetData gadgetData,
        GadgetBounds gadgetBounds,
        int stateIndex,
        Size baseSpriteSize,
        TribeManager tribeManager)
    {
        var animationController = GadgetAnimationControllerBuilder.BuildAnimationController(gadgetArchetypeData, gadgetData, gadgetBounds, stateIndex, baseSpriteSize, tribeManager);

        var state = gadgetArchetypeData.AllGadgetStateData[stateIndex];

        var hitBoxFilters = BuildHitBoxFilters(
            state,
            gadgetData,
            tribeManager);

        var hitBoxLookup = HitBoxBuilder.BuildHitBoxLookup(
            state,
            gadgetBounds);

        return new GadgetState(
            state.StateName,
            hitBoxFilters,
            hitBoxLookup,
            animationController);
    }

    private static LemmingHitBoxFilter[] BuildHitBoxFilters(
        GadgetStateArchetypeData state,
        GadgetData gadgetData,
        TribeManager tribeManager)
    {
        var result = Helpers.GetArrayForSize<LemmingHitBoxFilter>(state.HitBoxData.Length);

        for (var i = 0; i < result.Length; i++)
        {
            var hitBoxData = state.HitBoxData[i];

            GadgetActionBuilder.BuildGadgetActions(
                hitBoxData,
                out var onLemmingEnterActions,
                out var onLemmingPresentActions,
                out var onLemmingExitActions);

            result[i] = new LemmingHitBoxFilter(
                hitBoxData.SolidityType,
                hitBoxData.HitBoxBehaviour,
                HitBoxCriteriaBuilder.BuildLemmingCriteria(hitBoxData.HitBoxCriteria, gadgetData.OverrideHitBoxCriteriaData, tribeManager),
                onLemmingEnterActions,
                onLemmingPresentActions,
                onLemmingExitActions);
        }

        return result;
    }


    private static ResizeType GetResizeType(GadgetArchetypeData gadgetArchetypeData, GadgetData gadgetData)
    {
        return new DihedralTransformation(gadgetData.Orientation, gadgetData.FacingDirection).Transform(gadgetArchetypeData.ResizeType);
    }
}
