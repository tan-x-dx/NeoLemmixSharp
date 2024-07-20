using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

public sealed class TerrainData
{
    public int TerrainArchetypeId { get; set; }

    public int X { get; set; }
    public int Y { get; set; }

    public bool NoOverwrite { get; set; }
    public int RotNum { get; set; }
    public bool Flip { get; set; }
    public bool Erase { get; set; }

    public Color? Tint { get; set; }

    public string? GroupName { get; set; }

    public int? Width { get; set; }
    public int? Height { get; set; }

    public override string ToString()
    {
        var flipString = Flip ? "F" : string.Empty;
        var rotString = $"R{RotNum}";

        return $"X:{X},Y:{Y} - {rotString}{flipString}{rotString} - TerrainArchetypeId {TerrainArchetypeId}";
    }
}