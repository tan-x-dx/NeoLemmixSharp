using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets.BinaryLogic;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public static class LogicGateArchetypeBuilder
{
    public static AndGateGadget BuildAndGateGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetData gadgetData)
    {
        var inputNames = GetInputNames(gadgetData, 2, 256);

        Debug.Assert(inputNames.Length >= 2);

        var gadgetName = string.IsNullOrEmpty(gadgetData.OverrideName)
            ? gadgetArchetypeData.GadgetName
            : gadgetData.OverrideName;

        var gadgetBounds = GadgetBuildingHelpers.CreateGadgetBounds(gadgetArchetypeData, gadgetData);
        var states = BuildGadgetStates(gadgetArchetypeData, gadgetData, gadgetBounds);

        Debug.Assert(states.Length == 2);

        return new AndGateGadget(gadgetName, states, gadgetData.InitialStateId, inputNames)
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,

            CurrentGadgetBounds = gadgetBounds,

            IsFastForward = true
        };
    }

    public static OrGateGadget BuildOrGateGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetData gadgetData)
    {
        var inputNames = GetInputNames(gadgetData, 2, 256);

        Debug.Assert(inputNames.Length >= 2);

        var gadgetName = string.IsNullOrEmpty(gadgetData.OverrideName)
            ? gadgetArchetypeData.GadgetName
            : gadgetData.OverrideName;

        var gadgetBounds = GadgetBuildingHelpers.CreateGadgetBounds(gadgetArchetypeData, gadgetData);
        var states = BuildGadgetStates(gadgetArchetypeData, gadgetData, gadgetBounds);

        Debug.Assert(states.Length == 2);

        return new OrGateGadget(gadgetName, states, gadgetData.InitialStateId, inputNames)
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,

            CurrentGadgetBounds = gadgetBounds,

            IsFastForward = true
        };
    }

    public static NotGateGadget BuildNotGateGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetData gadgetData)
    {
        var inputNames = GetInputNames(gadgetData, 1, 1);

        Debug.Assert(inputNames.Length == 1);

        var gadgetName = string.IsNullOrEmpty(gadgetData.OverrideName)
            ? gadgetArchetypeData.GadgetName
            : gadgetData.OverrideName;

        var gadgetBounds = GadgetBuildingHelpers.CreateGadgetBounds(gadgetArchetypeData, gadgetData);
        var states = BuildGadgetStates(gadgetArchetypeData, gadgetData, gadgetBounds);

        Debug.Assert(states.Length == 2);

        return new NotGateGadget(gadgetName, states, gadgetData.InitialStateId, inputNames[0])
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,

            CurrentGadgetBounds = gadgetBounds,

            IsFastForward = true
        };
    }

    public static XorGateGadget BuildXorGateGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetData gadgetData)
    {
        var inputNames = GetInputNames(gadgetData, 2, 2);

        Debug.Assert(inputNames.Length == 2);

        var gadgetName = string.IsNullOrEmpty(gadgetData.OverrideName)
            ? gadgetArchetypeData.GadgetName
            : gadgetData.OverrideName;

        var gadgetBounds = GadgetBuildingHelpers.CreateGadgetBounds(gadgetArchetypeData, gadgetData);
        var states = BuildGadgetStates(gadgetArchetypeData, gadgetData, gadgetBounds);

        Debug.Assert(states.Length == 2);

        return new XorGateGadget(gadgetName, states, gadgetData.InitialStateId, inputNames[0], inputNames[1])
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,

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
        GadgetBounds gadgetBounds)
    {
        Debug.Assert(gadgetArchetypeData.AllGadgetStateData.Length == 2);

        var result = new GadgetState[2];

        result[0] = BuildGadgetState(gadgetArchetypeData.AllGadgetStateData[0], gadgetData, gadgetBounds, gadgetArchetypeData.BaseSpriteSize);
        result[1] = BuildGadgetState(gadgetArchetypeData.AllGadgetStateData[1], gadgetData, gadgetBounds, gadgetArchetypeData.BaseSpriteSize);

        return result;
    }

    private static GadgetState BuildGadgetState(
        GadgetStateArchetypeData state,
        GadgetData gadgetData,
        GadgetBounds gadgetBounds,
        Size baseSpriteSize)
    {
        var dihedralTransformation = new DihedralTransformation(gadgetData.Orientation, gadgetData.FacingDirection);
        var animationController = GadgetAnimationControllerBuilder.BuildAnimationController(state.AnimationLayerData, gadgetBounds, baseSpriteSize, dihedralTransformation);

        return new GadgetState(state.StateName, [], null, animationController);
    }
}
