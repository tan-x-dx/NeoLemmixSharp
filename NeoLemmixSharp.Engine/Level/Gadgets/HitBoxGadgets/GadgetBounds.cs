using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class GadgetBounds : IRectangularBounds
{
    public int X { get; set; }
    public int Y { get; set; }

    public int Width { get; set; }
    public int Height { get; set; }

    public LevelPosition TopLeftPosition => new(X, Y);
    public LevelPosition BottomRightPosition => new(X + Width, Y + Height);
    public LevelSize Size => new(Width, Height);
    public LevelRegion CurrentBounds => new(TopLeftPosition, Size);

    public void SetFrom(GadgetBounds other)
    {
        X = other.X;
        Y = other.Y;
        Width = other.Width;
        Height = other.Height;
    }

    public void SetPositionFrom(GadgetBounds other)
    {
        X = other.X;
        Y = other.Y;
    }

    public void SetSizeFrom(GadgetBounds other)
    {
        Width = other.Width;
        Height = other.Height;
    }

    public void SetPosition(LevelPosition position)
    {
        X = position.X;
        Y = position.Y;
    }

    public void ModifyPosition(LevelPosition delta)
    {
        X += delta.X;
        Y += delta.Y;
    }

    public void SetSize(LevelSize size)
    {
        Width = size.W;
        Height = size.H;
    }

    public void ModifySize(LevelSize delta)
    {
        Width += delta.W;
        Height += delta.H;
    }
}
