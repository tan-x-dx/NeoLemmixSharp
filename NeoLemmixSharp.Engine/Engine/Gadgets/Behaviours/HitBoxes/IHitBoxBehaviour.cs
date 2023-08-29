using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.Behaviours.HitBoxes;

public interface IHitBoxBehaviour
{
    bool MatchesLemming(
        Lemming lemming,
        LevelPosition levelPosition);
}