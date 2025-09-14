using NeoLemmixSharp.Common.Util;
using System.Numerics;

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

    public static bool ComparisonMatches<T>(this ComparisonType comparisonType, T value, T comparisonValue)
        where T : struct, IComparisonOperators<T, T, bool> => comparisonType switch
        {
            ComparisonType.EqualTo => value == comparisonValue,
            ComparisonType.NotEqualTo => value != comparisonValue,
            ComparisonType.LessThan => value < comparisonValue,
            ComparisonType.LessThanOrEqual => value <= comparisonValue,
            ComparisonType.GreaterThan => value > comparisonValue,
            ComparisonType.GreaterThanOrEqual => value >= comparisonValue,

            _ => Helpers.ThrowUnknownEnumValueException<ComparisonType, bool>(comparisonType),
        };
}
