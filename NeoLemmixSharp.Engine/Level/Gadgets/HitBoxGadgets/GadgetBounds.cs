using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class GadgetBounds : IRectangularBounds
{
    public LevelPosition Position { get; set; }

    public int Width { get; set; }
    public int Height { get; set; }

    public LevelSize Size => new(Width, Height);
    public LevelRegion CurrentBounds => new(Position, Size);

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

    public void SetPosition(LevelPosition position)
    {
        Position = position;
    }

    public void ModifyPosition(LevelPosition delta)
    {
        Position += delta;
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
