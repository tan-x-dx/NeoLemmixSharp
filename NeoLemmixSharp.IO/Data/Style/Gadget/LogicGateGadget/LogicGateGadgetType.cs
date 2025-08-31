using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.LogicGateGadget;

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
