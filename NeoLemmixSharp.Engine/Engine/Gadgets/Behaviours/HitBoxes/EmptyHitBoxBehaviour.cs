using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.Behaviours.HitBoxes;

public sealed class EmptyHitBoxBehaviour : IHitBoxBehaviour
{
    public static EmptyHitBoxBehaviour Instance { get; } = new();

    private EmptyHitBoxBehaviour()
    {
    }

    public bool MatchesLemming(Lemming lemming, LevelPosition levelPosition) => false;
}