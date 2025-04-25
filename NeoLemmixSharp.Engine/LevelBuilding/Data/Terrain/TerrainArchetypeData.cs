using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

public sealed class TerrainArchetypeData : ITerrainArchetypeData
{
    public required string Style { get; init; }
    public required string TerrainPiece { get; init; }

    public required bool IsSteel { get; init; }
    public required ResizeType ResizeType { get; init; }

    public required int DefaultWidth { get; init; }
    public required int DefaultHeight { get; init; }

    public required int NineSliceBottom { get; init; }
    public required int NineSliceLeft { get; init; }
    public required int NineSliceTop { get; init; }
    public required int NineSliceRight { get; init; }

    public ArrayWrapper2D<Color> TerrainPixelColorData { get; set; }

    public override string ToString()
    {
        return $"{Style}:{TerrainPiece}";
    }

    public bool IsTrivial()
    {
        return !(
            IsSteel ||
            ResizeType != ResizeType.None ||
            DefaultWidth > 0 ||
            DefaultHeight > 0);

    }

    public static TerrainArchetypeData CreateTrivialTerrainArchetypeData(
       string styleName,
       string pieceName) => new()
       {
           Style = styleName,
           TerrainPiece = pieceName,

           IsSteel = false,
           ResizeType = ResizeType.None,

           DefaultWidth = 0,
           DefaultHeight = 0,

           NineSliceBottom = 0,
           NineSliceLeft = 0,
           NineSliceTop = 0,
           NineSliceRight = 0
       };
}