using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public enum LemmingSolidityType
{
    NotSolid,
    Solid,
    Steel
}

public static class LemmingSolidityTypeHelpers
{
    private const int NumberOfEnumValues = 3;

    public static LemmingSolidityType GetEnumValue(int rawValue) => Helpers.GetEnumValue<LemmingSolidityType>(rawValue, NumberOfEnumValues);
}