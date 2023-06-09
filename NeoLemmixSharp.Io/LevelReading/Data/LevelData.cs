﻿using NeoLemmixSharp.Common.BoundaryBehaviours;

namespace NeoLemmixSharp.Io.LevelReading.Data;

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

    public BoundaryBehaviourType HorizontalBoundaryBehaviour { get; set; }
    public BoundaryBehaviourType VerticalBoundaryBehaviour { get; set; }

    public BoundaryBehaviourType HorizontalViewPortBehaviour { get; set; }
    public BoundaryBehaviourType VerticalViewPortBehaviour { get; set; }

    public SkillSetData SkillSetData { get; set; }
    public ThemeData ThemeData { get; } = new();
    public List<TerrainData> AllTerrainData { get; } = new();
    public List<TerrainGroup> AllTerrainGroups { get; } = new();
    public List<GadgetData> AllGadgetData { get; } = new();
}