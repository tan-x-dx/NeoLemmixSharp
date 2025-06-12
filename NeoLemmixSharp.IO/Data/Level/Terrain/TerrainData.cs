using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Style;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.IO.Data.Level.Terrain;

public sealed class TerrainData
{
    public required string? GroupName { get; init; }
    public required StyleIdentifier StyleIdentifier { get; init; }
    public required PieceIdentifier PieceIdentifier { get; init; }

    public required Point Position { get; init; }

    public required Orientation Orientation { get; init; }
    public required FacingDirection FacingDirection { get; init; }
    public required bool NoOverwrite { get; init; }
    public required bool Erase { get; init; }

    public required Color? Tint { get; init; }

    public required int? Width { get; init; }
    public required int? Height { get; init; }

    internal TerrainData()
    {
    }

    public StylePiecePair GetStylePiecePair() => new(StyleIdentifier, PieceIdentifier);
}