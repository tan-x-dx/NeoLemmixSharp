using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class HitBox
{
    private readonly ILevelRegion _levelRegion;
    private readonly ILemmingFilter[] _lemmingFilters;

    public HitBox(ILevelRegion levelRegion, ILemmingFilter[] lemmingFilters)
    {
        _levelRegion = levelRegion;
        _lemmingFilters = lemmingFilters;
    }

    public bool MatchesLemming(Lemming lemming)
    {
        foreach (var lemmingFilter in _lemmingFilters)
        {
            if (!lemmingFilter.MatchesLemming(lemming))
                return false;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MatchesPosition(LevelPosition levelPosition) => _levelRegion.ContainsPoint(levelPosition);

}