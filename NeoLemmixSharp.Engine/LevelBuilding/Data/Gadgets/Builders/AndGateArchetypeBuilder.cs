using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class AndGateArchetypeBuilder : IGadgetArchetypeBuilder
{
    public int GadgetBuilderId { get; }
    public required string Style { get; init; }
    public required string GadgetPiece { get; init; }

    public required SpriteData SpriteData { get; init; }

    public GadgetBase BuildGadget(
        GadgetSpriteBuilder gadgetSpriteBuilder,
        GadgetData gadgetData, 
        LemmingManager lemmingManager,
        TeamManager teamManager)
    {
        throw new NotImplementedException();
    }
}
