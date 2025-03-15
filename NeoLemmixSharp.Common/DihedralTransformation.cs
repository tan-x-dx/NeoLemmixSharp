using NeoLemmixSharp.Common.Rendering.Text;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common;

public readonly ref struct DihedralTransformation
{
    public static void Simplify(
        bool flipHorizontally,
        bool flipVertically,
        bool rotate,
        out int rotNum,
        out bool flip)
    {
        rotNum = rotate
            ? 1
            : 0;

        flip = flipHorizontally;
        if (flipVertically)
        {
            flip = !flip;
            rotNum += 2;
        }
    }

    private readonly Orientation _o;
    private readonly FacingDirection _f;

    public DihedralTransformation(Orientation orientation, FacingDirection facingDirection)
    {
        _o = new Orientation(orientation.RotNum);
        _f = new FacingDirection(facingDirection.Id);
    }

    public DihedralTransformation(int r, bool flip)
    {
        _o = new Orientation(r);
        _f = new FacingDirection(flip);
    }

    [SkipLocalsInit]
    public override string ToString()
    {
        Span<char> buffer = stackalloc char[10];

        buffer[0] = 'R';
        buffer[1] = 'o';
        buffer[2] = 't';
        buffer[3] = ' ';
        buffer[4] = TextRenderingHelpers.DigitToChar(_o.RotNum);
        buffer[5] = '|';

        int stringLength;
        if (_f == FacingDirection.Right)
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

    public LevelSize Transform(LevelSize levelSize)
    {
        Transform(levelSize.W, levelSize.H, levelSize.W, levelSize.H, out var x0, out var y0);
        return new LevelSize(x0, y0);
    }

    public void Transform(
        int x,
        int y,
        int w,
        int h,
        out int x0,
        out int y0)
    {
        var s = GetRotationCoefficients(out var a, out var b, ref w, ref h);
        s *= _f.Id;

        x0 = s + _f.DeltaX * (a * x - b * y + w);
        y0 = b * x + a * y + h;
    }

    private int GetRotationCoefficients(out int a, out int b, ref int w, ref int h)
    {
        var wTemp = w;
        var hTemp = h;
        switch (_o.RotNum)
        {
            case EngineConstants.DownOrientationRotNum:
                a = 1;
                b = 0;
                w = 0;
                h = 0;
                return wTemp;

            case EngineConstants.LeftOrientationRotNum:
                a = 0;
                b = 1;
                w = hTemp;
                h = 0;
                return hTemp;

            case EngineConstants.UpOrientationRotNum:
                a = -1;
                b = 0;
                return wTemp;

            case EngineConstants.RightOrientationRotNum:
                a = 0;
                b = -1;
                w = 0;
                h = wTemp;
                return hTemp;

            default:
                a = 0;
                b = 0;
                return Orientation.ThrowOrientationOutOfRangeException<int>(_o);
        }
    }
}