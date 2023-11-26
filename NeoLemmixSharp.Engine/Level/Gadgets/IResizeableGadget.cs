using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public interface IResizeableGadget : IRectangularBounds
{
    void Resize(int dw, int dh);
    void SetSize(int w, int h);
}