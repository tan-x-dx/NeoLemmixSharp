using NeoLemmixSharp.IO.Data.Level.Gadgets;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

public readonly struct HitBoxCriteriaData(LemmingCriteria lemmingCriteria, int itemId)
{
    public readonly LemmingCriteria LemmingCriteria = lemmingCriteria;
    public readonly int ItemId = itemId;
}
