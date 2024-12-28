using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public sealed class HitBox : IRectangularBounds
{
    public IHitBoxRegion HitBoxRegion { get; }
    private readonly ILemmingFilter[] _lemmingFilters;
    private readonly ILemmingFilter[] _lemmingSolidityFilters;

    public LevelPosition TopLeftPixel => HitBoxRegion.TopLeftPixel;
    public LevelPosition BottomRightPixel => HitBoxRegion.BottomRightPixel;
    public LevelPosition PreviousTopLeftPixel => HitBoxRegion.PreviousTopLeftPixel;
    public LevelPosition PreviousBottomRightPixel => HitBoxRegion.PreviousBottomRightPixel;

    public HitBox(
        IHitBoxRegion hitBoxRegion,
        ILemmingFilter[] lemmingFilters,
        ILemmingFilter[] lemmingSolidityFilters)
    {
        HitBoxRegion = hitBoxRegion;
        _lemmingFilters = lemmingFilters;
        _lemmingSolidityFilters = lemmingSolidityFilters;
    }

    public bool MatchesLemming(
        Lemming lemming,
        out LemmingSolidityType lemmingSolidityType)
    {
        var anchorPosition = lemming.LevelPosition;
        var footPosition = lemming.FootPosition;

        if (HitBoxRegion.ContainsPoint(anchorPosition) ||
            HitBoxRegion.ContainsPoint(footPosition))
        {
            var result = LemmingMatchesFilters(lemming);
            lemmingSolidityType = GetLemmingSolidity(lemming);
            return result;
        }

        lemmingSolidityType = LemmingSolidityType.NotSolid;
        return false;
    }

    private bool LemmingMatchesFilters(Lemming lemming)
    {
        foreach (var lemmingFilter in _lemmingFilters)
        {
            if (!lemmingFilter.MatchesLemming(lemming))
                return false;
        }

        return true;
    }

    private LemmingSolidityType GetLemmingSolidity(Lemming lemming)
    {
        foreach (var lemmingFilter in _lemmingSolidityFilters)
        {
            if (!lemmingFilter.MatchesLemming(lemming))
                return false;
        }

        return true;
    }
}