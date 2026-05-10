using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum TalismanRank
{
    Bronze,
    Silver,
    Gold,

    VALUE_MAX
}

public static class TalismanRankHelpers
{
    private const int NumberOfEnumValues = (int)TalismanRank.VALUE_MAX;

    public static TalismanRank GetEnumValue(uint rawValue) => Helpers.GetEnumValue<TalismanRank>(rawValue, NumberOfEnumValues);
}
