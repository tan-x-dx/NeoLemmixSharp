using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public interface IHitBoxRegion : IRectangularBounds
{
    bool ContainsPoint(LevelPosition levelPosition);

    void Move(int dx, int dy);
    void SetPosition(int x, int y);
}
