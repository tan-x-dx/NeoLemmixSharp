using System.Collections.Generic;

namespace NeoLemmixSharp.IO.LevelReading;

public sealed class TerrainGroup
{
    public int Id { get; }
    public string? GroupId { get; set; }

    public TerrainData DataPlaceholder { get; }

    public TerrainGroup(int id)
    {
        Id = id;

        DataPlaceholder = new TerrainData(id);
    }

    public List<TerrainData> TerrainDatas { get; } = new();
}