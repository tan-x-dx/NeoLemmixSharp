namespace NeoLemmixSharp.Engine.LevelBuilding.Data;

public sealed class TerrainGroup : IDisposable
{
    private bool _disposed;

    public string? GroupName { get; set; }
    public List<TerrainData> TerrainDatas { get; } = new();

    public bool IsPrimitive => TerrainDatas.TrueForAll(td => td.GroupName == null);

    public void Dispose()
    {
        if (_disposed)
            return;

        TerrainDatas.Clear();
        _disposed = true;
    }
}