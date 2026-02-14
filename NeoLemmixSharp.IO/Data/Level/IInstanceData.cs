using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Level;

public interface IInstanceData
{
    Point Position { get; set; }
    Size Size { get; }
    Orientation Orientation { get; set; }
    FacingDirection FacingDirection { get; set; }

    RectangularRegion GetBounds(Point anchorPosition);
}
