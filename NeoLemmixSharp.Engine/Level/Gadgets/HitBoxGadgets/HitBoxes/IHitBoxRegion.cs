using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public interface IHitBoxRegion
{
    LevelPosition Offset { get; }
    LevelSize BoundingBoxDimensions { get; }

    bool ContainsPoint(LevelPosition levelPosition);
}