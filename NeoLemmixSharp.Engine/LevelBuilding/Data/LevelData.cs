using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data;

public sealed class LevelData
{
    public string LevelTitle { get; set; } = null!;
    public string LevelAuthor { get; set; } = null!;
    public ulong LevelId { get; set; }
    public ulong Version { get; set; }
    public int LevelWidth { get; set; }
    public int LevelHeight { get; set; }
    public int LevelStartPositionX { get; set; }
    public int LevelStartPositionY { get; set; }
    public string LevelTheme { get; set; } = null!;
    public string LevelBackground { get; set; } = null!;

    public int NumberOfLemmings { get; set; }
    public int SaveRequirement { get; set; }
    public int? TimeLimit { get; set; }

    public BoundaryBehaviourType HorizontalBoundaryBehaviour { get; set; }
    public BoundaryBehaviourType VerticalBoundaryBehaviour { get; set; }

    public BoundaryBehaviourType HorizontalViewPortBehaviour { get; set; }
    public BoundaryBehaviourType VerticalViewPortBehaviour { get; set; }

    public List<SkillSetData> SkillSetData { get; } = new();
    public ThemeData ThemeData { get; } = new();
    public List<TerrainArchetypeData> TerrainArchetypeData { get; } = new();
    public List<TerrainData> AllTerrainData { get; } = new();
    public List<TerrainGroup> AllTerrainGroups { get; } = new();
    public List<HatchGroupData> AllHatchGroupData { get; } = new();
    public List<LemmingData> AllLemmingData { get; } = new();
    public List<GadgetArchetypeData> AllGadgetArchetypeData { get; } = new();
    public List<GadgetData> AllGadgetData { get; } = new();

    public bool LevelContainsAnyZombies()
    {
        //TODO implement this properly
        return false;
    }
}