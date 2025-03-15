using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public sealed class EmptyHitBoxRegion : IHitBoxRegion
{
    public static readonly EmptyHitBoxRegion Instance = new();
    public LevelRegion CurrentBounds => default;

    public bool ContainsPoint(LevelPosition levelPosition) => false;

    private EmptyHitBoxRegion()
    {
    }
}