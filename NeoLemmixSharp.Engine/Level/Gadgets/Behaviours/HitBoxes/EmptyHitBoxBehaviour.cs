using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.HitBoxes;

/// <summary>
/// A class that represents the "Do Nothing" behaviour of something that does not technically have a hit box
/// </summary>
public sealed class EmptyHitBoxBehaviour : IHitBoxBehaviour
{
    public static EmptyHitBoxBehaviour Instance { get; } = new();

    public bool IsEnabled
    {
        get => false;
        set { }
    }
    public bool InteractsWithLemming => false;

    private EmptyHitBoxBehaviour()
    {
    }

    public bool MatchesLemming(Lemming lemming) => false;
    public bool MatchesPosition(LevelPosition levelPosition) => false;

    public void OnLemmingInHitBox(Lemming lemming)
    {
    }

    public void OnLemmingNotInHitBox(Lemming lemming)
    {
    }
}