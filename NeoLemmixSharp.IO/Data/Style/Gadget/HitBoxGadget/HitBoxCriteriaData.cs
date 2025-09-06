using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;

public readonly struct HitBoxCriteriaData(LemmingCriteriaType lemmingCriteria, int itemId)
{
    public readonly LemmingCriteriaType LemmingCriteria = lemmingCriteria;
    public readonly int ItemId = itemId;
}
