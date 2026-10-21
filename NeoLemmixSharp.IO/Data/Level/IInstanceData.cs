using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Level;

public interface IInstanceData
{
    Point Position { get; set; }
    Size Size { get; }
    DihedralTransformation DihedralTransformation { get; set; }

    RectangularRegion GetBounds(Point anchorPosition);
}
