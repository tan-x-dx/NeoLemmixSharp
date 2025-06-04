using NeoLemmixSharp.Common;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Style.Terrain;

[DebuggerDisplay("{StyleIdentifier}:{PieceIdentifier}")]
public sealed class TerrainArchetypeData : ITerrainArchetypeData
{
    public required StyleIdentifier StyleIdentifier { get; init; }
    public required PieceIdentifier PieceIdentifier { get; init; }
    public required string Name { get; init; }

    public required bool IsSteel { get; init; }
    public required ResizeType ResizeType { get; init; }

    public required int DefaultWidth { get; init; }
    public required int DefaultHeight { get; init; }

    public required NineSliceData NineSliceData { get; init; }

    internal TerrainArchetypeData()
    {
    }

    internal bool IsTrivial()
    {
        return !(
            IsSteel ||
            ResizeType != ResizeType.None ||
            DefaultWidth > 0 ||
            DefaultHeight > 0);
    }

    internal static TerrainArchetypeData CreateTrivialTerrainArchetypeData(
       StyleIdentifier styleName,
       PieceIdentifier pieceName) => new()
       {
           StyleIdentifier = styleName,
           PieceIdentifier = pieceName,
           Name = string.Empty,

           IsSteel = false,
           ResizeType = ResizeType.None,

           DefaultWidth = 0,
           DefaultHeight = 0,

           NineSliceData = default
       };
}