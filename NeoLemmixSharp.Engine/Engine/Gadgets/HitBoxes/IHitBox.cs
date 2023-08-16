using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.HitBoxes;

public interface IHitBox
{
    bool MatchesLemming(
        Lemming lemming,
        LevelPosition levelPosition);
}