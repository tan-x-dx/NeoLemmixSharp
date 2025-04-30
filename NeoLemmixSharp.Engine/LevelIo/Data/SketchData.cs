using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.LevelIo.Data;

public sealed class SketchData
{
    public int Index { get; set; }

    public Point Position { get; set; }

    public Orientation Orientation { get; set; }
    public FacingDirection FacingDirection { get; set; }
}