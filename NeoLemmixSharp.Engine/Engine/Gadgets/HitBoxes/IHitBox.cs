using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.HitBoxes;

public interface IHitBox
{
    bool MatchesLemming(
        Lemming lemming,
        LevelPosition levelPosition);
}