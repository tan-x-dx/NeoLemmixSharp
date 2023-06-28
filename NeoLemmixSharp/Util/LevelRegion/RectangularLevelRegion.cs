namespace NeoLemmixSharp.Util.LevelRegion;

public sealed class RectangularLevelRegion : ILevelRegion
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

    public bool ContainsPoint(LevelPosition levelPosition) => X <= levelPosition.X &&
                                                              Y <= levelPosition.Y &&
                                                              levelPosition.X < X1 &&
                                                              levelPosition.Y < Y1;

    /*
    public bool Equals(RectangularLevelRegion other) => X == other.X &&
                                             Y == other.Y &&
                                             W == other.W &&
                                             H == other.H;

    public override bool Equals(object? obj) => obj is RectangularLevelRegion other &&
                                                X == other.X &&
                                                Y == other.Y &&
                                                W == other.W &&
                                                H == other.H;

    public override int GetHashCode() => 79427 * X +
                                         63391 * Y +
                                         24821 * W +
                                         39097 * H +
                                         57719;

    public static bool operator ==(RectangularLevelRegion left, RectangularLevelRegion right) => left.X == right.X &&
                                                                           left.Y == right.Y &&
                                                                           left.W == right.W &&
                                                                           left.H == right.H;

    public static bool operator !=(RectangularLevelRegion left, RectangularLevelRegion right) => left.X != right.X ||
                                                                           left.Y != right.Y ||
                                                                           left.W != right.W ||
                                                                           left.H != right.H;*/
}