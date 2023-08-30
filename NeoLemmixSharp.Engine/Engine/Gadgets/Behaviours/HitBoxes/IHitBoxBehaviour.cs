using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.Behaviours.HitBoxes;

public interface IHitBoxBehaviour
{
    bool IsEnabled { get; set; }
    bool InteractsWithLemming { get; }

    bool MatchesLemming(Lemming lemming);
    bool MatchesPosition(LevelPosition levelPosition);

    void OnLemmingEnterHitBox(Lemming lemming);
    void OnLemmingInHitBox(Lemming lemming);
}