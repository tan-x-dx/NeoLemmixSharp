using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum TribeSpriteLayerColorType
{
    NoRender,
    TrueColor,
    LemmingHairColor,
    LemmingSkinColor,
    LemmingBodyColor,
    LemmingFootColor,
    TribePaintColor,

    VALUE_MAX
}

public static class TribeSpriteLayerColorTypeHelpers
{
    private const int NumberOfEnumValues = (int)TribeSpriteLayerColorType.VALUE_MAX;

    public static TribeSpriteLayerColorType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<TribeSpriteLayerColorType>(rawValue, NumberOfEnumValues);
}
