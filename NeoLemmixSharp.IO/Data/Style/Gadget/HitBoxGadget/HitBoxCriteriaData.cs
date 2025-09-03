using NeoLemmixSharp.IO.Data.Level.Gadget;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;

public readonly struct HitBoxCriteriaData(LemmingCriteria lemmingCriteria, int itemId)
{
    public readonly LemmingCriteria LemmingCriteria = lemmingCriteria;
    public readonly int ItemId = itemId;
}
