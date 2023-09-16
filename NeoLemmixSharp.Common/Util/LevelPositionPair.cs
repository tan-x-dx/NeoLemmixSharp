namespace NeoLemmixSharp.Common.Util;

public readonly ref struct LevelPositionPair
{
    public readonly int P1X;
    public readonly int P1Y;

    public readonly int P2X;
    public readonly int P2Y;

    public LevelPositionPair(LevelPosition p1, LevelPosition p2)
    {
        P1X = Math.Min(p1.X, p2.X);
        P1Y = Math.Min(p1.Y, p2.Y);

        P2X = Math.Max(p1.X, p2.X);
        P2Y = Math.Max(p1.Y, p2.Y);
    }

    public LevelPositionPair(LevelPosition p1, LevelPosition p2, LevelPosition p3, LevelPosition p4)
    {
        var x0 = Math.Min(p1.X, p2.X);
        var y0 = Math.Min(p1.Y, p2.Y);

        var x1 = Math.Min(p3.X, p4.X);
        var y1 = Math.Min(p3.Y, p4.Y);

        P1X = Math.Min(x0, x1);
        P1Y = Math.Min(y0, y1);

        x0 = Math.Max(p1.X, p2.X);
        y0 = Math.Max(p1.Y, p2.Y);

        x1 = Math.Max(p3.X, p4.X);
        y1 = Math.Max(p3.Y, p4.Y);

        P2X = Math.Max(x0, x1);
        P2Y = Math.Max(y0, y1);
    }

    public LevelPosition GetTopLeftPosition() => new(P1X, P1Y);
    public LevelPosition GetBottomRightPosition() => new(P2X, P2Y);
}