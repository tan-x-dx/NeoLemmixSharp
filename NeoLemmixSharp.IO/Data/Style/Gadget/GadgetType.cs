using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public enum GadgetType
{
    HitBoxGadget,
    HatchGadget,

    GadgetMover,
    GadgetResizer,
    GadgetStateChanger,

    AndGate,
    OrGate,
    NotGate,
    XorGate
}

public static class GadgetTypeHelpers
{
    private const int NumberOfEnumValues = 9;

    public static GadgetType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<GadgetType>(rawValue, NumberOfEnumValues);
}
