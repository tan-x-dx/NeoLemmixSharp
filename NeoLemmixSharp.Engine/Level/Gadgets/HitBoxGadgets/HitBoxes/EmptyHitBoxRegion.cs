using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public sealed class EmptyHitBoxRegion : IResizableHitBoxRegion
{
    public static readonly EmptyHitBoxRegion Instance = new();

    LevelSize IHitBoxRegion.BoundingBoxDimensions => default;

    public bool ContainsPoint(LevelPosition levelPosition) => false;

    private EmptyHitBoxRegion()
    {
    }

    void IResizableHitBoxRegion.Resize(int dw, int dh)
    {
    }

    void IResizableHitBoxRegion.SetSize(int w, int h)
    {
    }
}