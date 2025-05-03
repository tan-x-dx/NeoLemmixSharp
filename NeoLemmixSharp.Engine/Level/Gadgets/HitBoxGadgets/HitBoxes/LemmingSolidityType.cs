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
    public static LemmingSolidityType GetEnumValue(int rawValue)
    {
        var enumValue = (LemmingSolidityType)rawValue;

        return enumValue switch
        {
            LemmingSolidityType.NotSolid => LemmingSolidityType.NotSolid,
            LemmingSolidityType.Solid => LemmingSolidityType.Solid,
            LemmingSolidityType.Steel => LemmingSolidityType.Steel,

            _ => Helpers.ThrowUnknownEnumValueException<LemmingSolidityType>(rawValue)
        };
    }
}