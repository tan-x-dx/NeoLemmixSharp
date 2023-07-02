namespace NeoLemmixSharp.Common.Util;

public sealed class DihedralTransformation
{
    private static DihedralTransformation[] Lookup { get; } = CreateLookup();

    private static DihedralTransformation[] CreateLookup()
    {
        var result = new DihedralTransformation[8];

        var r0 = new Rotation(0);
        var r1 = new Rotation(1);
        var r2 = new Rotation(2);
        var r3 = new Rotation(3);

        var f0 = new Reflection(false);
        var f1 = new Reflection(true);

        var dh0 = new DihedralTransformation(r0, f0);
        var dh1 = new DihedralTransformation(r1, f0);
        var dh2 = new DihedralTransformation(r2, f0);
        var dh3 = new DihedralTransformation(r3, f0);
        var dh4 = new DihedralTransformation(r0, f1);
        var dh5 = new DihedralTransformation(r1, f1);
        var dh6 = new DihedralTransformation(r2, f1);
        var dh7 = new DihedralTransformation(r3, f1);

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

    private readonly Rotation _rotation;
    private readonly Reflection _reflection;

    private int Key => _rotation.R | (_reflection.F << 2);

    private DihedralTransformation(Rotation rotation, Reflection reflection)
    {
        _rotation = rotation;
        _reflection = reflection;
    }

    public override string ToString() => $"{_rotation}|{_reflection}";

    public void Transform(
        int x,
        int y,
        int width,
        int height,
        out int x0,
        out int y0)
    {
        var m = _reflection.M;
        var w = _rotation.W(width, height);
        var h = _rotation.H(width, height);
        var s = _reflection.S(_rotation.Choose(width, height));
        x0 = s + m * (_rotation.A * x - _rotation.B * y + w);
        y0 = _rotation.B * x + _rotation.A * y + h;
    }

    private sealed class Rotation
    {
        public Rotation(int r)
        {
            R = r;

            switch (r)
            {
                case 0:
                    A = 1;
                    B = 0;
                    break;
                case 1:
                    A = 0;
                    B = 1;
                    break;
                case 2:
                    A = -1;
                    B = 0;
                    break;
                case 3:
                    A = 0;
                    B = -1;
                    break;
            }
        }

        public int R { get; }
        public int A { get; }
        public int B { get; }

        public int W(int w, int h) => R switch
        {
            0 => 0,
            1 => h,
            2 => w,
            3 => 0,
            _ => 0
        };

        public int H(int w, int h) => R switch
        {
            0 => 0,
            1 => 0,
            2 => h,
            3 => w,
            _ => 0
        };

        public int Choose(int w, int h) => (R & 1) == 0
            ? w
            : h;

        public override string ToString() => $"Rot {R}";
    }

    private sealed class Reflection
    {
        public int F { get; }
        public int M { get; }

        public Reflection(bool flip)
        {
            if (flip)
            {
                F = 1;
                M = -1;
            }
            else
            {
                F = 0;
                M = 1;
            }
        }

        public int S(int k) => F * k;

        public override string ToString() => F == 0
            ? string.Empty
            : "Flip";
    }
}