using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

public interface ITerrainArchetypeData
{
    PixelColorData TerrainPixelColorData { get; }
    bool IsSteel { get; }
}