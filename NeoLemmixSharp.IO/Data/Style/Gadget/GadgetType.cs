using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public enum BaseGadgetType
{
    HitBox,
    Hatch,
    Functional
}

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

    public static BaseGadgetType GetBaseGadgetType(this GadgetType subType) => subType switch
    {
        GadgetType.HitBoxGadget => BaseGadgetType.HitBox,
        GadgetType.HatchGadget => BaseGadgetType.Hatch,
        GadgetType.GadgetMover => BaseGadgetType.Functional,
        GadgetType.GadgetResizer => BaseGadgetType.Functional,
        GadgetType.GadgetStateChanger => BaseGadgetType.Functional,
        GadgetType.AndGate => BaseGadgetType.Functional,
        GadgetType.OrGate => BaseGadgetType.Functional,
        GadgetType.NotGate => BaseGadgetType.Functional,
        GadgetType.XorGate => BaseGadgetType.Functional,

        _ => Helpers.ThrowUnknownEnumValueException<GadgetType, BaseGadgetType>(subType)
    };
}
