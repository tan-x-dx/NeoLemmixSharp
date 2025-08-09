using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Data;

public sealed class NeoLemmixGadgetData
{
    public string Style { get; set; } = string.Empty;
    public string Piece { get; set; } = string.Empty;

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
    public int SkillId { get; set; }
    public int? SkillCount { get; set; }
    public int? LemmingCount { get; set; }

    public uint State { get; set; } = 1U << LemmingStateConstants.ActiveBitIndex;
}