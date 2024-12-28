using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public sealed class RectangularHitBoxRegion : IHitBoxRegion
{
    private LevelPosition _currentPosition;
    private LevelPosition _previousPosition;

    private int _currentW;
    private int _currentH;
    private int _previousW;
    private int _previousH;

    public int X => _currentPosition.X;
    public int Y => _currentPosition.Y;
    public int W => _currentW;
    public int H => _currentH;

    public int X1 => _currentPosition.X + _currentW;
    public int Y1 => _currentPosition.Y + _currentH;

    public RectangularHitBoxRegion(int x, int y, int w, int h)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(w);
        ArgumentOutOfRangeException.ThrowIfNegative(h);

        _currentPosition = LevelScreen.NormalisePosition(new LevelPosition(x, y));
        _previousPosition = _currentPosition;
        _currentW = w;
        _currentH = h;
        _previousW = w;
        _previousH = h;
    }

    public RectangularHitBoxRegion(LevelPosition p0, LevelPosition p1)
    {
        int x, y;

        if (p0.X < p1.X)
        {
            x = p0.X;
            _currentW = 1 + p1.X - p0.X;
        }
        else
        {
            x = p1.X;
            _currentW = 1 + p0.X - p1.X;
        }

        if (p0.Y < p1.Y)
        {
            y = p0.Y;
            _currentH = 1 + p1.Y - p0.Y;
        }
        else
        {
            y = p1.Y;
            _currentH = 1 + p0.Y - p1.Y;
        }

        _currentPosition = LevelScreen.NormalisePosition(new LevelPosition(x, y));
        _previousPosition = _currentPosition;
        _previousW = _currentW;
        _previousH = _currentH;
    }

    public bool ContainsPoint(LevelPosition levelPosition) => X <= levelPosition.X &&
                                                              Y <= levelPosition.Y &&
                                                              levelPosition.X < X1 &&
                                                              levelPosition.Y < Y1;

    public Rectangle ToRectangle() => new(X, Y, W, H);

    public void Move(int dx, int dy)
    {
        _previousPosition = _currentPosition;
        _currentPosition = LevelScreen.NormalisePosition(_currentPosition + new LevelPosition(dx, dy));

        _previousW = _currentW;
        _previousH = _currentH;
    }

    public void SetPosition(int x, int y)
    {
        _previousPosition = _currentPosition;
        _currentPosition = LevelScreen.NormalisePosition(new LevelPosition(x, y));

        _previousW = _currentW;
        _previousH = _currentH;
    }

    public void Resize(int dw, int dh)
    {
        _previousW = _currentW;
        _previousH = _currentH;
        _currentW = Math.Max(0, _currentW + dw);
        _currentH = Math.Max(0, _currentH + dh);

        _previousPosition = _currentPosition;
    }

    public void SetSize(int w, int h)
    {
        _previousW = _currentW;
        _previousH = _currentH;
        _currentW = Math.Max(0, w);
        _currentH = Math.Max(0, h);

        _previousPosition = _currentPosition;
    }

    public LevelPosition TopLeftPixel => _currentPosition;
    public LevelPosition BottomRightPixel => new(X1 - 1, Y1 - 1);
    public LevelPosition PreviousTopLeftPixel => _previousPosition;
    public LevelPosition PreviousBottomRightPixel => new(_previousPosition.X + _previousW - 1, _previousPosition.Y + _previousH - 1);

    public override string ToString()
    {
        Span<char> buffer = stackalloc char[2 + 11 + 1 + 11 + 5 + 10 + 3 + 10 + 1];

        var i = 0;
        buffer[i++] = '[';
        _currentPosition.TryFormat(buffer[i..], out var charsWritten);
        i += charsWritten;
        buffer[i++] = ',';
        buffer[i++] = ' ';
        buffer[i++] = 'W';
        buffer[i++] = ':';
        W.TryFormat(buffer[i..], out charsWritten);
        i += charsWritten;
        buffer[i++] = ' ';
        buffer[i++] = 'H';
        buffer[i++] = ':';
        H.TryFormat(buffer[i..], out charsWritten);
        i += charsWritten;
        buffer[i++] = ']';

        return buffer[..i].ToString();
    }
}
