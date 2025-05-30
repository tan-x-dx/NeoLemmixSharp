using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public sealed class GadgetBounds : IRectangularBounds
{
    public Point Position { get; set; }

    // Size data is raw ints, since it can technically be negative or zero
    public int Width { get; set; }
    public int Height { get; set; }

    public Size Size => new(Width, Height);
    public RectangularRegion CurrentBounds => new(Position, Size);
}
