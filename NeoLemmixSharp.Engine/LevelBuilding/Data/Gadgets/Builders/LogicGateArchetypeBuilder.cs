using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets.BinaryLogic;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class LogicGateArchetypeBuilder : IGadgetArchetypeBuilder
{
    public required string StyleName { get; init; }
    public required string PieceName { get; init; }
    public required LogicGateType LogicGateType { get; init; }

    public required SpriteData SpriteData { get; init; }

    public GadgetBase BuildGadget(
        GadgetSpriteBuilder gadgetSpriteBuilder,
        GadgetData gadgetData,
        LemmingManager lemmingManager,
        TeamManager teamManager)
    {
        return LogicGateType switch
        {
            LogicGateType.AndGate => CreateAndGateGadget(gadgetSpriteBuilder, gadgetData),
            LogicGateType.OrGate => CreateOrGateGadget(gadgetSpriteBuilder, gadgetData),
            LogicGateType.NotGate => CreateNotGateGadget(gadgetSpriteBuilder, gadgetData),
            LogicGateType.XorGate => CreateXorGateGadget(gadgetSpriteBuilder, gadgetData),

            _ => throw new ArgumentOutOfRangeException(nameof(LogicGateType), LogicGateType, "Unknown logic gate type")
        };
    }

    private static AndGateGadget CreateAndGateGadget(
        GadgetSpriteBuilder gadgetSpriteBuilder,
        GadgetData gadgetData)
    {
        if (gadgetData.InputNames.Length < 2)
            throw new ArgumentException("Expected at least 2 inputs!");

        return new AndGateGadget(null, null, gadgetData.InputNames)
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,

            CurrentGadgetBounds = new GadgetBounds(),
            PreviousGadgetBounds = new GadgetBounds(),

            IsFastForward = true
        };
    }

    private static OrGateGadget CreateOrGateGadget(
        GadgetSpriteBuilder gadgetSpriteBuilder,
        GadgetData gadgetData)
    {
        if (gadgetData.InputNames.Length < 2)
            throw new ArgumentException("Expected at least 2 inputs!");

        return new OrGateGadget(null, null, gadgetData.InputNames)
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,

            CurrentGadgetBounds = new GadgetBounds(),
            PreviousGadgetBounds = new GadgetBounds(),

            IsFastForward = true
        };
    }

    private static NotGateGadget CreateNotGateGadget(
        GadgetSpriteBuilder gadgetSpriteBuilder,
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
            PreviousGadgetBounds = new GadgetBounds(),

            IsFastForward = true
        };
    }

    private static XorGateGadget CreateXorGateGadget(
        GadgetSpriteBuilder gadgetSpriteBuilder,
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
            PreviousGadgetBounds = new GadgetBounds(),

            IsFastForward = true
        };
    }
}
