using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public interface IHitBoxRegion : IRectangularBounds
{
    bool ContainsPoint(LevelPosition levelPosition);
    bool ContainsPoints(LevelPosition p1, LevelPosition p2);
}