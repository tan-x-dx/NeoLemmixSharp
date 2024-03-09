using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

public sealed class TerrainArchetypeData
{
    public required int TerrainArchetypeId { get; init; }

    public required string? Style { get; init; }
    public required string? TerrainPiece { get; init; }

    public bool IsSteel { get; set; }
    public ResizeType ResizeType { get; set; }

    public int NineSliceRight { get; set; }
    public int NineSliceTop { get; set; }
    public int NineSliceLeft { get; set; }
    public int NineSliceBottom { get; set; }

    public int DefaultWidth { get; set; }
    public int DefaultHeight { get; set; }

    public PixelColorData TerrainPixelColorData { get; set; }

    public override string ToString()
    {
        return $"{Style}:{TerrainPiece}";
    }
}