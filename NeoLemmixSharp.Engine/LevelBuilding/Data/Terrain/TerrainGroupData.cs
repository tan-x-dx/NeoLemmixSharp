using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

public sealed class TerrainGroupData : ITerrainArchetypeData
{
    public string? GroupName { get; set; }
    public List<TerrainData> AllBasicTerrainData { get; } = new();

    public PixelColorData TerrainPixelColorData { get; set; }
    public bool IsSteel => false;
}