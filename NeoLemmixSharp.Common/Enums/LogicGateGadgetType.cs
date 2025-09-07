using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum LogicGateGadgetType
{
    AndGate,
    OrGate,
    NotGate,
    XorGate
}

public static class LogicGateGadgetTypeHelpers
{
    private const int NumberOfGadgetTypeEnumValues = 4;

    public static LogicGateGadgetType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LogicGateGadgetType>(rawValue, NumberOfGadgetTypeEnumValues);
}
