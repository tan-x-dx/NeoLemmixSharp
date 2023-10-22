namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class NeoLemmixGadgetData
{
    public string Style { get; set; }
    public string Piece { get; set; }

    public int X { get; set; }
    public int Y { get; set; }
    public bool NoOverwrite { get; set; }
    public bool OnlyOnTerrain { get; set; }
    public bool FlipVertical { get; set; }
    public bool FlipHorizontal { get; set; }
    public bool Rotate { get; set; }

    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? Speed { get; set; }
    public int? Angle { get; set; }
    public string? Skill { get; set; }
    public int? SkillCount { get; set; }

    public override string ToString()
    {
        var horzString = FlipHorizontal ? "H" : string.Empty;
        var vertString = FlipVertical ? "V" : string.Empty;
        var rotString = Rotate ? "R" : string.Empty;

        return $"X:{X},Y:{Y}{horzString}{vertString}{rotString} {Style}/{Piece}";
    }
}