using NeoLemmixSharp.Common;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Style.Terrain;

[DebuggerDisplay("{StyleName}:{PieceName}")]
public sealed class TerrainArchetypeData : ITerrainArchetypeData
{
    public required StyleIdentifier StyleName { get; init; }
    public required PieceIdentifier PieceName { get; init; }

    public required bool IsSteel { get; init; }
    public required ResizeType ResizeType { get; init; }

    public required int DefaultWidth { get; init; }
    public required int DefaultHeight { get; init; }

    public required int NineSliceBottom { get; init; }
    public required int NineSliceLeft { get; init; }
    public required int NineSliceTop { get; init; }
    public required int NineSliceRight { get; init; }

    public bool IsTrivial()
    {
        return !(
            IsSteel ||
            ResizeType != ResizeType.None ||
            DefaultWidth > 0 ||
            DefaultHeight > 0);
    }

    public static TerrainArchetypeData CreateTrivialTerrainArchetypeData(
       StylePiecePair stylePiecePair) => CreateTrivialTerrainArchetypeData(stylePiecePair.StyleName, stylePiecePair.PieceName);

    public static TerrainArchetypeData CreateTrivialTerrainArchetypeData(
       StyleIdentifier styleName,
       PieceIdentifier pieceName) => new()
       {
           StyleName = styleName,
           PieceName = pieceName,

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