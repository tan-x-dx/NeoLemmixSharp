using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.LevelIo.Data.Terrain;

public sealed class TerrainGroupData : ITerrainArchetypeData
{
    public string? GroupName { get; set; }
    public List<TerrainData> AllBasicTerrainData { get; } = new();

    public ArrayWrapper2D<Color> TerrainPixelColorData { get; set; }
    public bool IsSteel => false;
}