using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public abstract class HitBoxRegion : IRectangularBounds
{
    public abstract RectangularRegion CurrentBounds { get; }

    public abstract bool ContainsPoint(Point levelPosition);
    public abstract bool ContainsEitherPoint(Point p1, Point p2);
}