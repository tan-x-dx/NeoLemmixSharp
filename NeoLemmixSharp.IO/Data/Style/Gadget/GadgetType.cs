using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public enum GadgetType
{
    HitBoxGadget,
    HatchGadget,

    AndGate,
    OrGate,
    NotGate,
    XorGate
}

public static class GadgetTypeHelpers
{
    private const int NumberOfGadgetTypeEnumValues = 6;

    public static GadgetType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<GadgetType>(rawValue, NumberOfGadgetTypeEnumValues);
}
