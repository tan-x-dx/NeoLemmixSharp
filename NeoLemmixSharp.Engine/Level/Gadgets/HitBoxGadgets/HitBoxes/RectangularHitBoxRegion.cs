using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public sealed class RectangularHitBoxRegion : IResizableHitBoxRegion
{
    private int _width;
    private int _height;

    public RectangularHitBoxRegion(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public bool ContainsPoint(LevelPosition levelPosition) => _width > 0 &&
                                                              _height > 0 &&
                                                              (uint)levelPosition.X < (uint)_width &&
                                                              (uint)levelPosition.Y < (uint)_height;

    public void Resize(int dw, int dh)
    {
        _width += dw;
        _height += dh;
    }

    public void SetSize(int w, int h)
    {
        _width = w;
        _height = h;
    }
}
