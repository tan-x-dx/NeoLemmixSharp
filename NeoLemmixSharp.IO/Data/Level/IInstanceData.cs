using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Level;

public interface IInstanceData
{
    Point Position { get; set; }
    Size Size { get; }
    DihedralTransformation DihedralTransformation { get; }

    RectangularRegion GetBounds(Point anchorPosition);
}
