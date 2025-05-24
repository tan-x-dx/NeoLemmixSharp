using Microsoft.Xna.Framework;

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
}
