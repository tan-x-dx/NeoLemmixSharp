using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Level;

public interface IInstanceData
{
    Point Position { get; set; }
    Orientation Orientation { get; set; }
    FacingDirection FacingDirection { get; set; }
}
