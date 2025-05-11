using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelIo.Data.Style.Terrain;

namespace NeoLemmixSharp.Engine.LevelIo.Data.Level.Terrain;

public sealed class TerrainGroupData : ITerrainArchetypeData
{
    public string? GroupName { get; set; }
    public List<TerrainData> AllBasicTerrainData { get; } = new();

    public ArrayWrapper2D<Color> TerrainPixelColorData { get; set; }
    public bool IsSteel => false;
}