namespace NeoLemmixSharp.Util.LevelRegion;

public sealed class RectangularLevelRegion : IRectangularLevelRegion
{
    public int X { get; set; }
    public int Y { get; set; }
    public int W { get; set; }
    public int H { get; set; }

    public int X1 => X + W;
    public int Y1 => Y + H;

    public LevelPosition TopLeft => new(X, Y);

    public RectangularLevelRegion(int x, int y, int w, int h)
    {
        X = x;
        Y = y;
        W = w;
        H = h;
    }

    public RectangularLevelRegion(LevelPosition p0, LevelPosition p1)
    {
        if (p0.X < p1.X)
        {
            X = p0.X;
            W = 1 + p1.X - X;
        }
        else
        {
            X = p1.X;
            W = 1 + p0.X - X;
        }

        if (p0.Y < p1.Y)
        {
            Y = p0.Y;
            H = 1 + p1.Y - Y;
        }
        else
        {
            Y = p1.Y;
            H = 1 + p0.Y - Y;
        }
    }

    public bool ContainsPoint(LevelPosition levelPosition) => X <= levelPosition.X &&
                                                              Y <= levelPosition.Y &&
                                                              levelPosition.X < X1 &&
                                                              levelPosition.Y < Y1;
}