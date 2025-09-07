using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering.Criteria;

public abstract class LemmingCriterion : IComparable<LemmingCriterion>
{
    public LemmingCriteriaType Criterion { get; }

    protected LemmingCriterion(LemmingCriteriaType criterion)
    {
        Criterion = criterion;
    }

    [Pure]
    public abstract bool LemmingMatchesCriteria(Lemming lemming);

    public int CompareTo(LemmingCriterion? other)
    {
        if (other == null) return 1;

        return (int)Criterion - (int)other.Criterion;
    }
}
