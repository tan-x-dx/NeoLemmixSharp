using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.IO.Data.Level.Gadget;

public enum LemmingSolidityType
{
    NotSolid,
    Solid,
    Steel
}

public static class LemmingSolidityTypeHelpers
{
    private const int NumberOfEnumValues = 3;

    public static LemmingSolidityType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LemmingSolidityType>(rawValue, NumberOfEnumValues);
}