using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.IO.Data.Style.Theme;

public enum TribeSpriteLayerColorType
{
    NoRender,
    TrueColor,
    LemmingHairColor,
    LemmingSkinColor,
    LemmingBodyColor,
    LemmingFootColor,
    TribePaintColor
}

public static class TribeSpriteLayerColorTypeHelpers
{
    private const int NumberOfEnumValues = 7;

    public static TribeSpriteLayerColorType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<TribeSpriteLayerColorType>(rawValue, NumberOfEnumValues);
}
