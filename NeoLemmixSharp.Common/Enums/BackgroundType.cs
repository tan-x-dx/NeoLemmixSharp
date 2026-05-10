using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum BackgroundType
{
    NoBackgroundSpecified,
    SolidColorBackground,
    TextureBackground,

    VALUE_MAX
}

public static class BackgroundTypeHelpers
{
    private const uint NumberOfEnumValues = (uint)BackgroundType.VALUE_MAX;

    public static BackgroundType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<BackgroundType>(rawValue, NumberOfEnumValues);
}
