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
    public static BackgroundType GetBackgroundType(int rawValue)
    {
        var enumValue = (BackgroundType)rawValue;

        return enumValue switch
        {
            BackgroundType.NoBackgroundSpecified => BackgroundType.NoBackgroundSpecified,
            BackgroundType.SolidColorBackground => BackgroundType.SolidColorBackground,
            BackgroundType.TextureBackground => BackgroundType.TextureBackground,

            _ => Helpers.ThrowUnknownEnumValueException<BackgroundType>(rawValue)
        };
    }
}
