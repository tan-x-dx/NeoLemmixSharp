﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets.BinaryLogic;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public sealed class LogicGateArchetypeBuilder : IGadgetArchetypeBuilder
{
    public required StyleIdentifier StyleName { get; init; }
    public required PieceIdentifier PieceName { get; init; }
    public required LogicGateType LogicGateType { get; init; }

    public required SpriteArchetypeData SpriteData { get; init; }

    public GadgetBase BuildGadget(
        GadgetRendererBuilder gadgetSpriteBuilder,
        GadgetData gadgetData,
        LemmingManager lemmingManager,
        TribeManager tribeManager)
    {
        return LogicGateType switch
        {
            LogicGateType.AndGate => CreateAndGateGadget(gadgetSpriteBuilder, gadgetData),
            LogicGateType.OrGate => CreateOrGateGadget(gadgetSpriteBuilder, gadgetData),
            LogicGateType.NotGate => CreateNotGateGadget(gadgetSpriteBuilder, gadgetData),
            LogicGateType.XorGate => CreateXorGateGadget(gadgetSpriteBuilder, gadgetData),

            _ => Helpers.ThrowUnknownEnumValueException<LogicGateType, GadgetBase>(LogicGateType)
        };
    }

    private static AndGateGadget CreateAndGateGadget(
        GadgetRendererBuilder gadgetSpriteBuilder,
        GadgetData gadgetData)
    {
        if (gadgetData.InputNames.Length < 2)
            throw new ArgumentException("Expected at least 2 inputs!");

        return new AndGateGadget(null, null, gadgetData.InputNames)
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,

            CurrentGadgetBounds = new GadgetBounds(),

            IsFastForward = true
        };
    }

    private static OrGateGadget CreateOrGateGadget(
        GadgetRendererBuilder gadgetSpriteBuilder,
        GadgetData gadgetData)
    {
        if (gadgetData.InputNames.Length < 2)
            throw new ArgumentException("Expected at least 2 inputs!");

        return new OrGateGadget(null, null, gadgetData.InputNames)
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,

            CurrentGadgetBounds = new GadgetBounds(),

            IsFastForward = true
        };
    }

    private static NotGateGadget CreateNotGateGadget(
        GadgetRendererBuilder gadgetSpriteBuilder,
        GadgetData gadgetData)
    {
        if (gadgetData.InputNames.Length != 1)
            throw new InvalidOperationException("Expected precisely ONE input name!");

        var inputName = gadgetData.InputNames[0];
        return new NotGateGadget(null, null, inputName)
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,

            CurrentGadgetBounds = new GadgetBounds(),

            IsFastForward = true
        };
    }

    private static XorGateGadget CreateXorGateGadget(
        GadgetRendererBuilder gadgetSpriteBuilder,
        GadgetData gadgetData)
    {
        if (gadgetData.InputNames.Length != 2)
            throw new InvalidOperationException("Expected precisely TWO input names!");

        var inputName1 = gadgetData.InputNames[0];
        var inputName2 = gadgetData.InputNames[1];
        return new XorGateGadget(null, null, inputName1, inputName2)
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,

            CurrentGadgetBounds = new GadgetBounds(),

            IsFastForward = true
        };
    }
}
