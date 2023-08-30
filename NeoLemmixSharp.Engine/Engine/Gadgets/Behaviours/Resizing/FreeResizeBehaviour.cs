using NeoLemmixSharp.Common.Util.LevelRegion;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.Behaviours.Resizing;

public sealed class FreeResizeBehaviour : IResizeBehaviour
{
    private readonly RectangularLevelRegion _position;

    public FreeResizeBehaviour(RectangularLevelRegion position)
    {
        _position = position;
    }

    public void Resize(int dw, int dh)
    {
        _position.W += dw;
        _position.H += dh;
    }

    public void SetSize(int w, int h)
    {
        _position.W = w;
        _position.H = h;
    }
}