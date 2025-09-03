using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public sealed class GadgetBounds : IRectangularBounds
{
    public int X;
    public int Y;

    // Size data is raw ints, since it can technically be negative or zero
    public int Width;
    public int Height;

    public Point Position
    {
        get => new(X, Y);
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }

    public Size Size => new(Width, Height);
    public RectangularRegion CurrentBounds => new(Position, Size);
}
