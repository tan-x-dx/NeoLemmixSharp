using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

public sealed class TerrainData
{
    public required string? GroupName { get; init; }
    public required string Style { get; init; }
    public required string TerrainPiece { get; init; }

    public required int X { get; init; }
    public required int Y { get; init; }

    public required bool NoOverwrite { get; init; }
    public required int RotNum { get; init; }
    public required bool Flip { get; init; }
    public required bool Erase { get; init; }

    public required Color? Tint { get; init; }

    public required int? Width { get; init; }
    public required int? Height { get; init; }

    public LevelData.StylePiecePair GetStylePiecePair() => new(Style, TerrainPiece);

    public override string ToString()
    {
        var flipString = Flip ? "F" : string.Empty;
        var rotString = $"R{RotNum}";

        return $"X:{X},Y:{Y} - {rotString}{flipString}{rotString}";
    }
}