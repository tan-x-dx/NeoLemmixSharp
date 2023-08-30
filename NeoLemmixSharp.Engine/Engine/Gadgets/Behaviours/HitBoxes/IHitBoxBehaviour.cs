using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.Behaviours.HitBoxes;

public interface IHitBoxBehaviour
{
    bool IsEnabled { get; set; }
    bool InteractsWithLemming { get; }

    bool MatchesLemming(Lemming lemming);
    bool MatchesPosition(LevelPosition levelPosition);

    void OnLemmingInHitBox(Lemming lemming);
    void OnLemmingNotInHitBox(Lemming lemming);
}