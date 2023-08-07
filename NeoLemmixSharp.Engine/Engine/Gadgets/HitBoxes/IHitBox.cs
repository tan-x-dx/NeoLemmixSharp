using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.HitBoxes;

public interface IHitBox
{
    bool TryGetGadgetThatMatchesTypeAndOrientation(
        Lemming lemming,
        LevelPosition levelPosition);
}