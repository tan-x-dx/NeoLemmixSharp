namespace NeoLemmixSharp.Common.Util;

public readonly ref struct LevelPositionPair
{
    private readonly int _p1X;
    private readonly int _p1Y;
    private readonly int _p2X;
    private readonly int _p2Y;

    public LevelPositionPair(LevelPosition p1, LevelPosition p2)
    {
        _p1X = Math.Min(p1.X, p2.X);
        _p2X = Math.Max(p1.X, p2.X);

        _p1Y = Math.Min(p1.Y, p2.Y);
        _p2Y = Math.Max(p1.Y, p2.Y);
    }

    public LevelPosition GetTopLeftPosition() => new(_p1X, _p1Y);
    public LevelPosition GetBottomRightPosition() => new(_p2X, _p2Y);
}