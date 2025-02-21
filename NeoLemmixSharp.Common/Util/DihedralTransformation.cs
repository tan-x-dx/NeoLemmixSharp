﻿using NeoLemmixSharp.Common.Rendering.Text;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public readonly ref struct DihedralTransformation
{
    public static (int RotNum, bool Flip) Simplify(
        bool flipHorizontally,
        bool flipVertically,
        bool rotate)
    {
        var rotNum = rotate
              ? 1
              : 0;

        if (flipVertically)
        {
            flipHorizontally = !flipHorizontally;
            rotNum += 2;
        }

        return (rotNum, flipHorizontally);
    }

    private readonly int _r;
    private readonly int _a;
    private readonly int _b;

    private readonly int _f;
    private readonly int _m;

    public DihedralTransformation()
        : this(0, false)
    {
    }

    public DihedralTransformation(int r, bool flip)
    {
        _r = r & 3;

        switch (_r)
        {
            case 0:
                _a = 1;
                _b = 0;
                break;
            case 1:
                _a = 0;
                _b = 1;
                break;
            case 2:
                _a = -1;
                _b = 0;
                break;
            case 3:
                _a = 0;
                _b = -1;
                break;
        }

        if (flip)
        {
            _f = 1;
            _m = -1;
        }
        else
        {
            _f = 0;
            _m = 1;
        }
    }

    [SkipLocalsInit]
    public override string ToString()
    {
        Span<char> buffer = stackalloc char[10];

        buffer[0] = 'R';
        buffer[1] = 'o';
        buffer[2] = 't';
        buffer[3] = ' ';
        buffer[4] = TextRenderingHelpers.DigitToChar(_r);
        buffer[5] = '|';

        int stringLength;
        if (_f == 0)
        {
            stringLength = 6;
        }
        else
        {
            stringLength = 10;
            buffer[6] = 'F';
            buffer[7] = 'l';
            buffer[8] = 'i';
            buffer[9] = 'p';
        }

        return buffer[..stringLength].ToString();
    }

    public LevelPosition Transform(
        LevelPosition position,
        int width,
        int height)
    {
        Transform(position.X, position.Y, width, height, out var x0, out var y0);
        return new LevelPosition(x0, y0);
    }

    public void Transform(
        int x,
        int y,
        int width,
        int height,
        out int x0,
        out int y0)
    {
        var w = W(width, height);
        var h = H(width, height);
        var s = _f * Choose(width, height);

        x0 = s + _m * (_a * x - _b * y + w);
        y0 = _b * x + _a * y + h;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int W(int w, int h) => _r switch
    {
        1 => h,
        2 => w,
        _ => 0
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int H(int w, int h) => _r switch
    {
        2 => h,
        3 => w,
        _ => 0
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Choose(int w, int h) => (_r & 1) != 0
        ? h
        : w;
}