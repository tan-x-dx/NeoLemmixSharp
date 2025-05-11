using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.LevelIo.Data.Style;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.Engine.LevelIo.Data.Level.Terrain;

public sealed class TerrainData
{
    public required string? GroupName { get; init; }
    public required StyleIdentifier StyleName { get; init; }
    public required PieceIdentifier PieceName { get; init; }

    public required Point Position { get; init; }

    public required Orientation Orientation { get; init; }
    public required FacingDirection FacingDirection { get; init; }
    public required bool NoOverwrite { get; init; }
    public required bool Erase { get; init; }

    public required Color? Tint { get; init; }

    public required int? Width { get; init; }
    public required int? Height { get; init; }

    public StylePiecePair GetStylePiecePair() => new(StyleName, PieceName);
}