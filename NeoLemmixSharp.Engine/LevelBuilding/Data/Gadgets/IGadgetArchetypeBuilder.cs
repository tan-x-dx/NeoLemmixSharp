﻿using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;

public interface IGadgetArchetypeBuilder
{
    string StyleName { get; }
    string PieceName { get; }

    SpriteData SpriteData { get; }

    GadgetBase BuildGadget(
        GadgetSpriteBuilder gadgetSpriteBuilder,
        GadgetData gadgetData,
        LemmingManager lemmingManager,
        TeamManager teamManager);
}