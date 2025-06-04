using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.IO.Data.Style.Theme;

public readonly struct TribeColorData(
    Color hairColor,
    Color permanentSkillHairColor,
    Color skinColor,
    Color zombieSkinColor,
    Color bodyColor,
    Color permanentSkillBodyColor,
    Color neutralBodyColor,
    Color acidLemmingFootColor,
    Color waterLemmingFootColor,
    Color paintColor)
{
    public readonly Color HairColor = hairColor;
    public readonly Color PermanentSkillHairColor = permanentSkillHairColor;

    public readonly Color SkinColor = skinColor;
    public readonly Color ZombieSkinColor = zombieSkinColor;

    public readonly Color BodyColor = bodyColor;
    public readonly Color PermanentSkillBodyColor = permanentSkillBodyColor;
    public readonly Color NeutralBodyColor = neutralBodyColor;

    public readonly Color AcidLemmingFootColor = acidLemmingFootColor;
    public readonly Color WaterLemmingFootColor = waterLemmingFootColor;

    public readonly Color PaintColor = paintColor;

    public Color GetFromTribeSpriteLayerColorType(TribeSpriteLayerColorType tribeSpriteLayerColorType) => tribeSpriteLayerColorType switch
    {
        TribeSpriteLayerColorType.NoRender => Color.Transparent,
        TribeSpriteLayerColorType.TrueColor => Color.White,
        TribeSpriteLayerColorType.LemmingHairColor => HairColor,
        TribeSpriteLayerColorType.LemmingSkinColor => SkinColor,
        TribeSpriteLayerColorType.LemmingBodyColor => BodyColor,
        TribeSpriteLayerColorType.LemmingFootColor => WaterLemmingFootColor,
        TribeSpriteLayerColorType.TribePaintColor => PaintColor,

        _ => Helpers.ThrowUnknownEnumValueException<TribeSpriteLayerColorType, Color>(tribeSpriteLayerColorType)
    };
}
