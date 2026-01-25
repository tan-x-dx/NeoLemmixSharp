using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Level;

public sealed class SketchInstanceData : IInstanceData
{
    public int Index { get; set; }
    public Point Position { get; set; }
    public Orientation Orientation { get; set; }
    public FacingDirection FacingDirection { get; set; }

    internal SketchInstanceData()
    {
    }
}
