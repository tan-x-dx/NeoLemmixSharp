using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LogicalNotLemmingCriterion : ILemmingCriterion
{
    private readonly ILemmingCriterion _filter;

    public LogicalNotLemmingCriterion(ILemmingCriterion filter)
    {
        _filter = filter;
    }

    public bool LemmingMatchesCriteria(Lemming lemming)
    {
        return !_filter.LemmingMatchesCriteria(lemming);
    }
}