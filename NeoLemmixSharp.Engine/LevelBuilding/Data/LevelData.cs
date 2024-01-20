﻿using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data;

public sealed class LevelData
{
    public string LevelTitle { get; set; }
    public string LevelAuthor { get; set; }
    public ulong LevelId { get; set; }
    public ulong Version { get; set; }
    public int LevelWidth { get; set; }
    public int LevelHeight { get; set; }
    public int LevelStartPositionX { get; set; }
    public int LevelStartPositionY { get; set; }
    public string LevelTheme { get; set; }
    public string LevelBackground { get; set; }

    public int NumberOfLemmings { get; set; }
    public int SaveRequirement { get; set; }
    public int? TimeLimit { get; set; }

    public BoundaryBehaviourType HorizontalBoundaryBehaviour { get; set; } = BoundaryBehaviourType.Void;
    public BoundaryBehaviourType VerticalBoundaryBehaviour { get; set; } = BoundaryBehaviourType.Void;

    public BoundaryBehaviourType HorizontalViewPortBehaviour { get; set; } = BoundaryBehaviourType.Void;
    public BoundaryBehaviourType VerticalViewPortBehaviour { get; set; } = BoundaryBehaviourType.Void;

    public List<SkillSetData> SkillSetData { get; } = new();
    public ThemeData ThemeData { get; } = new();
    public List<TerrainData> AllTerrainData { get; } = new();
    public List<TerrainGroup> AllTerrainGroups { get; } = new();
    public List<HatchGroupData> AllHatchGroupData { get; } = new();
    public List<GadgetArchetypeData> AllGadgetArchetypeData { get; } = new();
    public List<GadgetData> AllGadgetData { get; } = new();

    public bool LevelContainsAnyZombies()
    {
        //TODO implement this properly
        return false;
    }
}