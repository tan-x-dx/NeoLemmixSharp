using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public sealed class EmptyHitBoxRegion : IHitBoxRegion
{
    public static readonly EmptyHitBoxRegion Instance = new();

    LevelPosition IHitBoxRegion.Offset => new();
    LevelSize IHitBoxRegion.BoundingBoxDimensions => new();

    public bool ContainsPoint(LevelPosition levelPosition) => false;

    private EmptyHitBoxRegion()
    {
    }
}