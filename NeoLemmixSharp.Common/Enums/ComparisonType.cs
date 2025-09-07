using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum ComparisonType
{
    EqualTo,
    NotEqualTo,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
}

public static class ComparisonTypeHelpers
{
    private const int NumberOfEnumValues = 6;

    public static ComparisonType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<ComparisonType>(rawValue, NumberOfEnumValues);
}
