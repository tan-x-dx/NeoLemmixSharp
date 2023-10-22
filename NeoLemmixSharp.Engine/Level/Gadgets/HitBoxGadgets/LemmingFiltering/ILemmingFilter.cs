using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public interface ILemmingFilter
{
    bool MatchesLemming(Lemming lemming);
}