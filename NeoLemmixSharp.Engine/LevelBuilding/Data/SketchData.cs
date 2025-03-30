using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data;

public sealed class SketchData
{
    public int Index { get; set; }

    public LevelPosition Position { get; set; }

    public Orientation Orientation { get; set; }
    public FacingDirection FacingDirection { get; set; }
}