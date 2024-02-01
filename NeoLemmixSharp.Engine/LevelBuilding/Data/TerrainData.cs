namespace NeoLemmixSharp.Engine.LevelBuilding.Data;

public sealed class TerrainData
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool NoOverwrite { get; set; }
    public bool FlipVertical { get; set; }
    public bool FlipHorizontal { get; set; }
    public bool Erase { get; set; }
    public bool Rotate { get; set; }

    public uint? Tint { get; set; }

    public string? GroupName { get; set; }
    public int TerrainArchetypeId { get; set; }

    public override string ToString()
    {
        var horzString = FlipHorizontal ? "H" : string.Empty;
        var vertString = FlipVertical ? "V" : string.Empty;
        var rotString = Rotate ? "R" : string.Empty;

        return $"X:{X},Y:{Y}{horzString}{vertString}{rotString} - TerrainArchetypeId {TerrainArchetypeId}";
    }
}