using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public sealed class EmptyHitBoxRegion : IHitBoxRegion
{
    public static readonly EmptyHitBoxRegion Instance = new();
    public Region CurrentBounds => default;

    public bool ContainsPoint(Point levelPosition) => false;
    public bool ContainsPoints(Point p1, Point p2) => false;

    private EmptyHitBoxRegion()
    {
    }
}