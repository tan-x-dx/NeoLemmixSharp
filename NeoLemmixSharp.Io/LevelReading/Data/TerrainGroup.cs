namespace NeoLemmixSharp.Io.LevelReading.Data;

public sealed class TerrainGroup : IDisposable
{
    private bool _disposed;

    public string? GroupId { get; set; }
    public List<TerrainData> TerrainDatas { get; } = new();

    public bool IsPrimitive => TerrainDatas.All(td => td.GroupId == null);

    public void Dispose()
    {
        if (_disposed)
            return;

        TerrainDatas.Clear();
        _disposed = true;
    }
}