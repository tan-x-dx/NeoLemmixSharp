using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

public interface ITerrainArchetypeData
{
    ArrayWrapper2D<Color> TerrainPixelColorData { get; }
    bool IsSteel { get; }
}