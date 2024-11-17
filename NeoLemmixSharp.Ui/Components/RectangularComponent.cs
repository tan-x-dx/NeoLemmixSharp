using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Ui.Components;

public abstract class RectangularComponent : Component, IColorable
{
    private int _width, _height;

    private ColorPacket _colourPacket;

    private Action? _resizeAction = null;

    protected RectangularComponent(int x, int y, int width, int height)
        : base(x, y)
    {
        Width = width;
        Height = height;
        _colourPacket = UiConstants.RectangularButtonDefaultColours;
    }

    protected RectangularComponent(int x, int y, int width, int height, string? label)
        : base(x, y, label)
    {
        Width = width;
        Height = height;
        _colourPacket = UiConstants.RectangularButtonDefaultColours;
    }

    public override int Width
    {
        get { return _width; }
        set
        {
            _width = value;

            _resizeAction?.Invoke();
        }
    }

    public override int Height
    {
        get { return _height; }
        set
        {
            _height = value;

            _resizeAction?.Invoke();
        }
    }

    public ColorPacket Colors
    {
        get => _colourPacket;
        set => _colourPacket = value;
    }

    public void SetSize(int w, int h)
    {
        _width = w;
        _height = h;

        _resizeAction?.Invoke();
    }

    public void SetDimensions(int x, int y, int width, int height)
    {
        SetLocation(x, y);
        SetSize(width, height);
    }

    public override bool ContainsPoint(LevelPosition position)
    {
        return position.X >= Left &&
               position.Y >= Top &&
               position.X < Right &&
               position.Y < Bottom;
    }

    public void SetResizeAction(Action action) => _resizeAction = action;
}
