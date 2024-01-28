using NeoLemmixSharp.Engine.LevelBuilding.Sprites;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data;

public sealed class TerrainArchetypeData : IStyleArchetypeData
{
    public required int TerrainArchetypeId { get; init; }

    public required string? Style { get; init; }
    public required string? TerrainPiece { get; init; }
    string? IStyleArchetypeData.Piece => TerrainPiece;
    public required bool IsSteel { get; init; }

    public PixelColorData TerrainPixelColorData { get; set; }

    public override string ToString()
    {
        return $"{Style}:{TerrainPiece}";
    }
}