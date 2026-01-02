using NeoLemmixSharp.Common;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.IO.Data.Level.Terrain;

public sealed class TerrainData
{
    public required string? GroupName { get; set; }
    public required StyleIdentifier StyleIdentifier { get; init; }
    public required PieceIdentifier PieceIdentifier { get; init; }

    public required Point Position { get; set; }

    public required Orientation Orientation { get; set; }
    public required FacingDirection FacingDirection { get; set; }
    public required bool NoOverwrite { get; set; }
    public required bool Erase { get; set; }

    public required Color? Tint { get; set; }

    public required int? Width { get; set; }
    public required int? Height { get; set; }

    public TerrainData()
    {
    }

    public StylePiecePair GetStylePiecePair() => new(StyleIdentifier, PieceIdentifier);
}