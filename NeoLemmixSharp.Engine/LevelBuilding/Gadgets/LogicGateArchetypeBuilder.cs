using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets.BinaryLogic;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public static class LogicGateArchetypeBuilder
{
    public static GadgetBase BuildGadget(
        LogicGateType logicGateType,
        GadgetArchetypeData gadgetArchetypeData,
        GadgetData gadgetData,
        LemmingManager lemmingManager,
        TribeManager tribeManager)
    {
        return logicGateType switch
        {
            LogicGateType.AndGate => CreateAndGateGadget(gadgetArchetypeData, gadgetData),
            LogicGateType.OrGate => CreateOrGateGadget(gadgetArchetypeData, gadgetData),
            LogicGateType.NotGate => CreateNotGateGadget(gadgetArchetypeData, gadgetData),
            LogicGateType.XorGate => CreateXorGateGadget(gadgetArchetypeData, gadgetData),

            _ => Helpers.ThrowUnknownEnumValueException<LogicGateType, GadgetBase>(logicGateType)
        };
    }

    private static AndGateGadget CreateAndGateGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetData gadgetData)
    {
        if (gadgetData.OverrideInputNames.Length < 2)
            throw new ArgumentException("Expected at least 2 inputs!");

        return null;

        /*
        return new AndGateGadget(null, null, gadgetData.InputNames)
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,

            CurrentGadgetBounds = new GadgetBounds(),

            IsFastForward = true
        };*/
    }

    private static OrGateGadget CreateOrGateGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetData gadgetData)
    {
        if (gadgetData.OverrideInputNames.Length < 2)
            throw new ArgumentException("Expected at least 2 inputs!");

        return null;

        /*
        return new OrGateGadget(null, null, gadgetData.InputNames)
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,

            CurrentGadgetBounds = new GadgetBounds(),

            IsFastForward = true
        };*/
    }

    private static NotGateGadget CreateNotGateGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetData gadgetData)
    {
        if (gadgetData.OverrideInputNames.Length != 1)
            throw new InvalidOperationException("Expected precisely ONE input name!");

        return null;

        /*
        var inputName = gadgetData.InputNames[0];
        return new NotGateGadget(null, null, inputName)
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,

            CurrentGadgetBounds = new GadgetBounds(),

            IsFastForward = true
        };*/
    }

    private static XorGateGadget CreateXorGateGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetData gadgetData)
    {
        if (gadgetData.OverrideInputNames.Length != 2)
            throw new InvalidOperationException("Expected precisely TWO input names!");

        return null;

        /*
        var inputName1 = gadgetData.InputNames[0];
        var inputName2 = gadgetData.InputNames[1];
        return new XorGateGadget(null, null, inputName1, inputName2)
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,

            CurrentGadgetBounds = new GadgetBounds(),

            IsFastForward = true
        };*/
    }
}
