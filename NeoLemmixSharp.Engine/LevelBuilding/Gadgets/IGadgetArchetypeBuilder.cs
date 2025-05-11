using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.LevelIo.Data.Level.Gadgets;
using NeoLemmixSharp.Engine.LevelIo.Data.Style;
using NeoLemmixSharp.Engine.LevelIo.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public interface IGadgetArchetypeBuilder
{
    StyleIdentifier StyleName { get; }
    PieceIdentifier PieceName { get; }

    SpriteArchetypeData SpriteData { get; }

    GadgetBase BuildGadget(
        GadgetRendererBuilder gadgetSpriteBuilder,
        GadgetData gadgetData,
        LemmingManager lemmingManager,
        TeamManager teamManager);
}