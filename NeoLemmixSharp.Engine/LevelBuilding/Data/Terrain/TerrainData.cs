﻿using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

public sealed class TerrainData
{
    public required string? GroupName { get; init; }
    public required string Style { get; init; }
    public required string TerrainPiece { get; init; }

    public required int X { get; init; }
    public required int Y { get; init; }

    public required bool NoOverwrite { get; init; }
    public required Orientation Orientation { get; init; }
    public required FacingDirection FacingDirection { get; init; }
    public required bool Erase { get; init; }

    public required Color? Tint { get; init; }

    public required int? Width { get; init; }
    public required int? Height { get; init; }

    public StylePiecePair GetStylePiecePair() => new(Style, TerrainPiece);

    public override string ToString()
    {
        var flipString = FacingDirection == FacingDirection.Left ? "F" : string.Empty;
        var rotString = $"R{Orientation}";

        return $"X:{X},Y:{Y} - {rotString}{flipString}{rotString}";
    }
}