using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum LemmingSolidityType
{
    NotSolid,
    Solid,
    Steel,

    VALUE_MAX
}

public static class LemmingSolidityTypeHelpers
{
    private const int NumberOfEnumValues = (int)LemmingSolidityType.VALUE_MAX;

    public static LemmingSolidityType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LemmingSolidityType>(rawValue, NumberOfEnumValues);
}