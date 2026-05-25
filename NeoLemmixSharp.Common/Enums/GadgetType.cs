using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum GadgetType
{
    HitBoxGadget,
    HatchGadget,

    LogicGate,
    Counter,
    LevelTimerObserver,

    VALUE_MAX
}

public static class GadgetTypeHelpers
{
    private const int NumberOfGadgetTypeEnumValues = (int)GadgetType.VALUE_MAX;

    public static GadgetType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<GadgetType>(rawValue, NumberOfGadgetTypeEnumValues);
}
