using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public abstract class LemmingCriterion : IComparable<LemmingCriterion>
{
    public LemmingCriteria Criterion { get; }

    protected LemmingCriterion(LemmingCriteria criterion)
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
