using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public static class LemmingActionBounds
{
    public static readonly RectangularRegion StandardLemmingBounds = new(new Point(-3, 10), new Point(3, 0));

    public static readonly RectangularRegion BlockerActionBounds = new(new Point(-6, 10), new Point(6, 0));
    public static readonly RectangularRegion ClimberActionBounds = new(new Point(-6, 10), new Point(0, 0));
    public static readonly RectangularRegion DiggerLemmingBounds = new(new Point(-3, 5), new Point(3, 0));
    public static readonly RectangularRegion DisarmerLemmingBounds = new(new Point(-3, 8), new Point(3, 0));
    public static readonly RectangularRegion GliderActionBounds = new(new Point(-3, 12), new Point(3, 0));
    public static readonly RectangularRegion HoisterActionBounds = new(new Point(-5, 10), new Point(1, 1));
    public static readonly RectangularRegion JumperActionBounds = new(new Point(-1, 9), new Point(3, 0));
    public static readonly RectangularRegion MinerActionBounds = new(new Point(-2, 10), new Point(4, 0));
    public static readonly RectangularRegion PlatformerActionBounds = new(new Point(-3, 5), new Point(3, 0));
    public static readonly RectangularRegion ReacherActionBounds = new(new Point(-3, 9), new Point(3, 0));
    public static readonly RectangularRegion ShimmierActionBounds = new(new Point(-3, 9), new Point(3, 2));
    public static readonly RectangularRegion SplatterActionBounds = new(new Point(-3, 6), new Point(3, 0));
    public static readonly RectangularRegion SwimmerActionBounds = new(new Point(-7, 4), new Point(5, 0));
    public static readonly RectangularRegion VaporiserActionBounds = new(new Point(-3, 12), new Point(3, 2));
}
