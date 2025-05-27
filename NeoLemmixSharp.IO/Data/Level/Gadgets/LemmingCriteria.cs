using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.IO.Data.Level.Gadgets;

public enum LemmingCriteria
{
    LemmingOrientation,
    LemmingFacingDirection,
    LemmingAction,
    LemmingState,
    LemmingTribe
}

public static class LemmingCriteriaHelpers
{
    private const int NumberOfEnumValues = 5;

    public static LemmingCriteria GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LemmingCriteria>(rawValue, NumberOfEnumValues);
}
