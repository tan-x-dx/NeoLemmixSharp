using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class TrivialCriterion : ILemmingCriterion
{
    public static readonly TrivialCriterion Instance = new();
    public static readonly ILemmingCriterion[] TrivialCriteria = [Instance];

    private TrivialCriterion()
    {
    }

    public bool LemmingMatchesCriteria(Lemming lemming) => true;
}
