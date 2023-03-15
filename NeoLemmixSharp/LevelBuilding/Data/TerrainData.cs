namespace NeoLemmixSharp.LevelBuilding.Data;

public sealed class TerrainData
{
    public int Id { get; }

    public int X { get; set; }
    public int Y { get; set; }
    public bool NoOverwrite { get; set; }
    public bool FlipVertical { get; set; }
    public bool FlipHorizontal { get; set; }
    public bool Erase { get; set; }
    public bool Rotate { get; set; }

    public string? GroupId { get; set; }
    public string? Style { get; set; }
    public string? TerrainName { get; set; }

    public TerrainData(int id)
    {
        Id = id;
    }

    public override string ToString()
    {
        var horzString = FlipHorizontal ? "H" : string.Empty;
        var vertString = FlipVertical ? "V" : string.Empty;
        var rotString = Rotate ? "R" : string.Empty;
        var dataString = GroupId ?? $"{Style}/{TerrainName}";

        return $"X:{X},Y:{Y}{horzString}{vertString}{rotString} {dataString}";
    }
}