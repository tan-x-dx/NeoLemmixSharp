using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class GadgetBounds : IRectangularBounds
{
    public Point Position { get; set; }

    // Size data is raw ints, since it can technically be negative or zero
    // The LevelSize type will deal with this, giving us the behaviour we want
    public int Width { get; set; }
    public int Height { get; set; }

    public Size Size => new(Width, Height);
    public Region CurrentBounds => new(Position, Size);

    public GadgetBounds()
    {
    }

    public GadgetBounds(GadgetBounds other) => SetFrom(other);

    public void SetFrom(GadgetBounds other)
    {
        Position = other.Position;
        Width = other.Width;
        Height = other.Height;
    }

    public void SetPositionFrom(GadgetBounds other)
    {
        Position = other.Position;
    }

    public void SetSizeFrom(GadgetBounds other)
    {
        Width = other.Width;
        Height = other.Height;
    }

    public void SetPosition(Point position)
    {
        Position = position;
    }

    public void ModifyPosition(Point delta)
    {
        Position += delta;
    }

    public void SetSize(Size size)
    {
        Width = size.W;
        Height = size.H;
    }

    public void ModifySize(Size delta)
    {
        Width += delta.W;
        Height += delta.H;
    }
}
