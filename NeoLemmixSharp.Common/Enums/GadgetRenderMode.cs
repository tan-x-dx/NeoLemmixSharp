using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum GadgetRenderMode
{
    NoRender,
    BehindTerrain,
    InFrontOfTerrain,
    OnlyOnTerrain,

    VALUE_MAX
}

public static class GadgetRenderModeHelpers
{
    private const int NumberOfEnumValues = (int)GadgetRenderMode.VALUE_MAX;

    public static GadgetRenderMode GetEnumValue(uint rawValue) => Helpers.GetEnumValue<GadgetRenderMode>(rawValue, NumberOfEnumValues);
}
