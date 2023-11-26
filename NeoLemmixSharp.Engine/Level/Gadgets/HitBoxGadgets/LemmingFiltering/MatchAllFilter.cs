using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class MatchAllFilter : ILemmingFilter
{
    public static MatchAllFilter Instance { get; } = new();

    private MatchAllFilter()
    {
    }

    public bool MatchesLemming(Lemming lemming) => true;
}