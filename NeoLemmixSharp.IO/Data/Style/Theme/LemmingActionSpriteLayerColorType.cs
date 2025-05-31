using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.IO.Data.Style.Theme;

public enum LemmingActionSpriteLayerColorType
{
    TrueColor,
    LemmingHairColor,
    LemmingSkinColor,
    LemmingBodyColor,
    LemmingFootColor,
    TribePaintColor
}

public static class LemmingActionSpriteLayerColorTypeHelpers
{
    private const int NumberOfEnumValues = 6;

    public static LemmingActionSpriteLayerColorType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LemmingActionSpriteLayerColorType>(rawValue, NumberOfEnumValues);
}
