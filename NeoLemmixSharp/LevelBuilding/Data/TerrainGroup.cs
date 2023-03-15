using NeoLemmixSharp.IO.LevelReading;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoLemmixSharp.LevelBuilding.Data;

public sealed class TerrainGroup : IDisposable
{
    private bool _disposed;

    public string? GroupId { get; set; }
    public List<TerrainData> TerrainDatas { get; } = new();
    public TerrainTextureBundle? TerrainTextureBundle { get; set; }

    public bool IsPrimitive => TerrainDatas.All(td => td.GroupId == null);

    public void Dispose()
    {
        if (_disposed)
            return;

        TerrainTextureBundle?.Dispose();
        TerrainTextureBundle = null;
        TerrainDatas.Clear();
        _disposed = true;
    }
}