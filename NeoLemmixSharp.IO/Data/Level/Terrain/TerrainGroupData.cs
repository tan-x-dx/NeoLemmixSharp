using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Style.Terrain;

namespace NeoLemmixSharp.IO.Data.Level.Terrain;

public sealed class TerrainGroupData : ITerrainArchetypeData
{
    public string? GroupName { get; set; }
    public List<TerrainInstanceData> AllBasicTerrainData { get; } = new();

    public ArrayWrapper2D<Color> TerrainPixelColorData { get; set; }
    public bool IsSteel => false;

    internal TerrainGroupData()
    {
    }
}
