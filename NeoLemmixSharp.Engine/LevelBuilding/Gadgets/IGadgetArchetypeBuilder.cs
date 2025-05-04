using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.LevelIo.Data.Gadgets;
using NeoLemmixSharp.Engine.LevelIo.Data.Gadgets.ArchetypeData;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public interface IGadgetArchetypeBuilder
{
    string StyleName { get; }
    string PieceName { get; }

    SpriteArchetypeData SpriteData { get; }

    GadgetBase BuildGadget(
        GadgetRendererBuilder gadgetSpriteBuilder,
        GadgetData gadgetData,
        LemmingManager lemmingManager,
        TeamManager teamManager);
}