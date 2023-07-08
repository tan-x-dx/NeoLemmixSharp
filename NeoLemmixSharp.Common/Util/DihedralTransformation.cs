using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public sealed class DihedralTransformation
{
    private static DihedralTransformation[] Lookup { get; } = CreateLookup();

    private static DihedralTransformation[] CreateLookup()
    {
        var result = new DihedralTransformation[8];

        var dh0 = new DihedralTransformation(0, false);
        var dh1 = new DihedralTransformation(1, false);
        var dh2 = new DihedralTransformation(2, false);
        var dh3 = new DihedralTransformation(3, false);
        var dh4 = new DihedralTransformation(0, true);
        var dh5 = new DihedralTransformation(1, true);
        var dh6 = new DihedralTransformation(2, true);
        var dh7 = new DihedralTransformation(3, true);

        result[dh0.Key] = dh0;
        result[dh1.Key] = dh1;
        result[dh2.Key] = dh2;
        result[dh3.Key] = dh3;
        result[dh4.Key] = dh4;
        result[dh5.Key] = dh5;
        result[dh6.Key] = dh6;
        result[dh7.Key] = dh7;

        return result;
    }

    public static DihedralTransformation GetForTransformation(
        bool flipHorizontally,
        bool flipVertically,
        bool rotate)
    {
        var rotNum = rotate
            ? 1
            : 0;

        int flipNum;
        if (flipVertically)
        {
            flipNum = flipHorizontally
                ? 0
                : 4;
            rotNum += 2;
        }
        else
        {
            flipNum = flipHorizontally
                ? 4
                : 0;
        }

        var key = flipNum | rotNum;
        return Lookup[key];
    }

    public static DihedralTransformation GetForTransformation(
        bool flipHorizontally,
        int rotNum)
    {
        var flipNum = flipHorizontally
            ? 4
            : 0;

        var key = flipNum | rotNum;
        return Lookup[key];
    }

    private readonly int _r;
    private readonly int _a;
    private readonly int _b;

    private readonly int _f;
    private readonly int _m;

    private int Key => _r | (_f << 2);

    private DihedralTransformation(int r, bool flip)
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

    public override string ToString() => $"Rot {_r}|{FlipString}";

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string FlipString => _f == 0 ? string.Empty : "Flip";

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
        0 => 0,
        1 => h,
        2 => w,
        3 => 0,
        _ => 0
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int H(int w, int h) => _r switch
    {
        0 => 0,
        1 => 0,
        2 => h,
        3 => w,
        _ => 0
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Choose(int w, int h) => (_r & 1) == 1
        ? h
        : w;
}