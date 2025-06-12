﻿using NeoLemmixSharp.Common;
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

    public required Size DefaultSize { get; init; }

    public required RectangularRegion NineSliceData { get; init; }

    internal TerrainArchetypeData()
    {
    }

    internal bool IsTrivial()
    {
        return !(
            IsSteel ||
            ResizeType != ResizeType.None ||
            DefaultSize.W > 0 ||
            DefaultSize.H > 0);
    }

    internal static TerrainArchetypeData CreateTrivialTerrainArchetypeData(
       StyleIdentifier styleIdentifier,
       PieceIdentifier pieceIdentifier) => new()
       {
           StyleIdentifier = styleIdentifier,
           PieceIdentifier = pieceIdentifier,
           Name = string.Empty,

           IsSteel = false,
           ResizeType = ResizeType.None,

           DefaultSize = default,
           NineSliceData = default
       };
}