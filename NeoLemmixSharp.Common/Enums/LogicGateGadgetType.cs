using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum LogicGateGadgetType
{
    AndGate,
    OrGate,
    NotGate,
    XorGate,

    VALUE_MAX
}

public static class LogicGateGadgetTypeHelpers
{
    private const int NumberOfGadgetTypeEnumValues = (int)LogicGateGadgetType.VALUE_MAX;

    public static LogicGateGadgetType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LogicGateGadgetType>(rawValue, NumberOfGadgetTypeEnumValues);
}
