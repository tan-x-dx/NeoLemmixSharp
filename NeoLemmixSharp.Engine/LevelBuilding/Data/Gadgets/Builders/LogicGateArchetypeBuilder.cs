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
        return new AndGateGadget(gadgetData.Id, gadgetData.Orientation, new GadgetBounds(), gadgetData.InputNames);
    }

    private static OrGateGadget CreateOrGateGadget(
        GadgetSpriteBuilder gadgetSpriteBuilder,
        GadgetData gadgetData)
    {
        return new OrGateGadget(gadgetData.Id, gadgetData.Orientation, new GadgetBounds(), gadgetData.InputNames);
    }

    private static NotGateGadget CreateNotGateGadget(
        GadgetSpriteBuilder gadgetSpriteBuilder,
        GadgetData gadgetData)
    {
        if (gadgetData.InputNames.Length != 1)
            throw new InvalidOperationException("Expected precisely ONE input name!");

        var input = gadgetData.InputNames[0];
        return new NotGateGadget(gadgetData.Id, gadgetData.Orientation, new GadgetBounds(), input);
    }

    private static XorGateGadget CreateXorGateGadget(
        GadgetSpriteBuilder gadgetSpriteBuilder,
        GadgetData gadgetData)
    {
        if (gadgetData.InputNames.Length != 2)
            throw new InvalidOperationException("Expected precisely TWO input names!");

        var input1 = gadgetData.InputNames[0];
        var input2 = gadgetData.InputNames[1];
        return new XorGateGadget(gadgetData.Id, gadgetData.Orientation, new GadgetBounds(), input1, input2);
    }
}
