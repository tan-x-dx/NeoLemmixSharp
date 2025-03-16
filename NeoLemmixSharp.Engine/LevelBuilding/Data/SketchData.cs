using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data;

public sealed class SketchData
{
    public int Index { get; set; }

    public int X { get; set; }
    public int Y { get; set; }

    public Orientation Orientation { get; set; }
    public FacingDirection FacingDirection { get; set; }
}