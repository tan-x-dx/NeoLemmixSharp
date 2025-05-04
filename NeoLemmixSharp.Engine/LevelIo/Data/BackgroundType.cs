using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.LevelIo.Data;

public enum BackgroundType : uint
{
    NoBackgroundSpecified = 0x00,
    SolidColorBackground = 0x01,
    TextureBackground = 0x02
}

public static class BackgroundTypeHelpers
{
    private const int NumberOfEnumValues = 3;

    public static BackgroundType GetEnumValue(int rawValue) => Helpers.GetEnumValue<BackgroundType>(rawValue, NumberOfEnumValues);
}
