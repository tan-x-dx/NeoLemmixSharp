using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class HitBox
{
    public static readonly HitBox Empty = new(EmptyHitBoxRegion.Instance, Array.Empty<ILemmingFilter>());

    private readonly IHitBoxRegion _hitBoxRegion;
    private readonly ILemmingFilter[] _lemmingFilters;

    public HitBox(IHitBoxRegion hitBoxRegion, ILemmingFilter[] lemmingFilters)
    {
        _hitBoxRegion = hitBoxRegion;
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
    public bool MatchesPosition(LevelPosition levelPosition) => _hitBoxRegion.ContainsPoint(levelPosition);

}