using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.HitBoxes;

public interface IHitBoxBehaviour
{
    bool IsEnabled { get; set; }
    bool InteractsWithLemming { get; }

    bool MatchesLemming(Lemming lemming);
    bool MatchesPosition(LevelPosition levelPosition);

    void OnLemmingInHitBox(Lemming lemming);
    void OnLemmingNotInHitBox(Lemming lemming);
}