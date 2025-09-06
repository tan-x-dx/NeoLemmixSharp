using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum GadgetType
{
    HitBoxGadget,
    HatchGadget,

    LogicGate,
    LevelTimerObserver
}

public static class GadgetTypeHelpers
{
    private const int NumberOfGadgetTypeEnumValues = 6;

    public static GadgetType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<GadgetType>(rawValue, NumberOfGadgetTypeEnumValues);
}
