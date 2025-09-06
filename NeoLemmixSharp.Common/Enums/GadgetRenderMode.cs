using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum GadgetRenderMode
{
    NoRender,
    BehindTerrain,
    InFrontOfTerrain,
    OnlyOnTerrain
}

public static class GadgetRenderModeHelpers
{
    private const int NumberOfEnumValues = 4;

    public static GadgetRenderMode GetEnumValue(uint rawValue) => Helpers.GetEnumValue<GadgetRenderMode>(rawValue, NumberOfEnumValues);
}
