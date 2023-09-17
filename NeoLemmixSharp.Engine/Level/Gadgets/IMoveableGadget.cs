using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public interface IMoveableGadget : IRectangularBounds
{
    void Move(int dx, int dy);
    void SetPosition(int x, int y);
}