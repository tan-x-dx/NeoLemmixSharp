using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public interface ILemmingCriterion
{
    [Pure]
    bool LemmingMatchesCriteria(Lemming lemming);
}