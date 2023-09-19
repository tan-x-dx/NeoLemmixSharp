﻿using NeoLemmixSharp.Common.BoundaryBehaviours;

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
    public int MaxSpawnInterval { get; set; }

    public bool SuperLemmingMode { get; set; }

    public BoundaryBehaviourType HorizontalBoundaryBehaviour { get; set; } = BoundaryBehaviourType.Wrap;
    public BoundaryBehaviourType VerticalBoundaryBehaviour { get; set; } = BoundaryBehaviourType.Wrap;

    public BoundaryBehaviourType HorizontalViewPortBehaviour { get; set; } = BoundaryBehaviourType.Wrap;
    public BoundaryBehaviourType VerticalViewPortBehaviour { get; set; } = BoundaryBehaviourType.Wrap;

    public List<SkillSetData> SkillSetData { get; } = new();
    public ThemeData ThemeData { get; } = new();
    public List<TerrainData> AllTerrainData { get; } = new();
    public List<TerrainGroup> AllTerrainGroups { get; } = new();
    public List<GadgetData> AllGadgetData { get; } = new();
}