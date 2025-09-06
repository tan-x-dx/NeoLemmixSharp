using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum TalismanRank
{
    Bronze,
    Silver,
    Gold
}

public static class TalismanRankHelpers
{
    private const int NumberOfEnumValues = 3;

    public static TalismanRank GetEnumValue(uint rawValue) => Helpers.GetEnumValue<TalismanRank>(rawValue, NumberOfEnumValues);
}
