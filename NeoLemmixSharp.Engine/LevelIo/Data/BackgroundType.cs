using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.LevelIo.Data;

public enum BackgroundType
{
    NoBackgroundSpecified,
    SolidColorBackground,
    TextureBackground,
}

public static class BackgroundTypeHelpers
{
    private const int NumberOfEnumValues = 3;

    public static BackgroundType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<BackgroundType>(rawValue, NumberOfEnumValues);
}
