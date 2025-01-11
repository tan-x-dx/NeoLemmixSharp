using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public interface ILemmingCriterion
{
    bool LemmingMatchesCriteria(Lemming lemming);
}