﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets.BinaryLogic;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public static class LogicGateArchetypeBuilder
{
    public static AndGateGadget BuildAndGateGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetData gadgetData,
        TribeManager tribeManager)
    {
        var inputNames = GetInputNames(gadgetData, 2, 256);

        Debug.Assert(inputNames.Length >= 2);

        var gadgetName = GadgetBuildingHelpers.GetGadgetName(gadgetArchetypeData, gadgetData);
        var gadgetBounds = GadgetBuildingHelpers.CreateGadgetBounds(gadgetArchetypeData, gadgetData);
        var states = BuildGadgetStates(gadgetArchetypeData, gadgetData, gadgetBounds, tribeManager);

        Debug.Assert(states.Length == EngineConstants.NumberOfAllowedStatesForFunctionalGadgets);

        return new AndGateGadget(gadgetName, states, gadgetData.InitialStateId, inputNames)
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,
            FacingDirection = gadgetData.FacingDirection,

            CurrentGadgetBounds = gadgetBounds,

            IsFastForward = true
        };
    }

    public static OrGateGadget BuildOrGateGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetData gadgetData,
        TribeManager tribeManager)
    {
        var inputNames = GetInputNames(gadgetData, 2, 256);

        Debug.Assert(inputNames.Length >= 2);

        var gadgetName = GadgetBuildingHelpers.GetGadgetName(gadgetArchetypeData, gadgetData);
        var gadgetBounds = GadgetBuildingHelpers.CreateGadgetBounds(gadgetArchetypeData, gadgetData);
        var states = BuildGadgetStates(gadgetArchetypeData, gadgetData, gadgetBounds, tribeManager);

        Debug.Assert(states.Length == EngineConstants.NumberOfAllowedStatesForFunctionalGadgets);

        return new OrGateGadget(gadgetName, states, gadgetData.InitialStateId, inputNames)
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,
            FacingDirection = gadgetData.FacingDirection,

            CurrentGadgetBounds = gadgetBounds,

            IsFastForward = true
        };
    }

    public static NotGateGadget BuildNotGateGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetData gadgetData,
        TribeManager tribeManager)
    {
        var inputNames = GetInputNames(gadgetData, 1, 1);

        Debug.Assert(inputNames.Length == 1);

        var gadgetName = GadgetBuildingHelpers.GetGadgetName(gadgetArchetypeData, gadgetData);
        var gadgetBounds = GadgetBuildingHelpers.CreateGadgetBounds(gadgetArchetypeData, gadgetData);
        var states = BuildGadgetStates(gadgetArchetypeData, gadgetData, gadgetBounds, tribeManager);

        Debug.Assert(states.Length == EngineConstants.NumberOfAllowedStatesForFunctionalGadgets);

        return new NotGateGadget(gadgetName, states, gadgetData.InitialStateId, inputNames[0])
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,
            FacingDirection = gadgetData.FacingDirection,

            CurrentGadgetBounds = gadgetBounds,

            IsFastForward = true
        };
    }

    public static XorGateGadget BuildXorGateGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetData gadgetData,
        TribeManager tribeManager)
    {
        var inputNames = GetInputNames(gadgetData, 2, 2);

        Debug.Assert(inputNames.Length == 2);

        var gadgetName = GadgetBuildingHelpers.GetGadgetName(gadgetArchetypeData, gadgetData);
        var gadgetBounds = GadgetBuildingHelpers.CreateGadgetBounds(gadgetArchetypeData, gadgetData);
        var states = BuildGadgetStates(gadgetArchetypeData, gadgetData, gadgetBounds, tribeManager);

        Debug.Assert(states.Length == EngineConstants.NumberOfAllowedStatesForFunctionalGadgets);

        return new XorGateGadget(gadgetName, states, gadgetData.InitialStateId, inputNames[0], inputNames[1])
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,
            FacingDirection = gadgetData.FacingDirection,

            CurrentGadgetBounds = gadgetBounds,

            IsFastForward = true
        };
    }

    private static ReadOnlySpan<GadgetInputName> GetInputNames(
        GadgetData gadgetData,
        int minExpectedInputCount,
        int maxExpectedInputCount)
    {
        var numberOfInputs = gadgetData.GetProperty(GadgetProperty.NumberOfInputs);

        ArgumentOutOfRangeException.ThrowIfLessThan(numberOfInputs, minExpectedInputCount);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(numberOfInputs, maxExpectedInputCount);

        if (gadgetData.OverrideInputNames.Length > 0)
        {
            Debug.Assert(gadgetData.OverrideInputNames.Length == numberOfInputs);
            return gadgetData.OverrideInputNames;
        }

        return GadgetBuildingHelpers.GetInputNamesForCount(numberOfInputs);
    }

    private static GadgetState[] BuildGadgetStates(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetData gadgetData,
        GadgetBounds gadgetBounds,
        TribeManager tribeManager)
    {
        Debug.Assert(gadgetArchetypeData.AllGadgetStateData.Length == EngineConstants.NumberOfAllowedStatesForFunctionalGadgets);

        var result = new GadgetState[EngineConstants.NumberOfAllowedStatesForFunctionalGadgets];

        result[0] = BuildGadgetState(gadgetArchetypeData, gadgetData, gadgetBounds, 0, gadgetArchetypeData.BaseSpriteSize, tribeManager);
        result[1] = BuildGadgetState(gadgetArchetypeData, gadgetData, gadgetBounds, 1, gadgetArchetypeData.BaseSpriteSize, tribeManager);

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

        return new GadgetState(gadgetArchetypeData.AllGadgetStateData[stateIndex].StateName, animationController);
    }
}
