using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public interface IHitBoxRegion
{
    LevelSize BoundingBoxDimensions { get; }

    bool ContainsPoint(LevelPosition levelPosition);
}

public interface IResizableHitBoxRegion : IHitBoxRegion
{
    void Resize(int dw, int dh);
    void SetSize(int w, int h);
}