using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Animations;

public static class TeamColors
{
    public enum ColorType
    {
        JustWhite,
        GetHairColor,
        GetSkinColor,
        GetBodyColor
    }

    public static TeamColorChooser GetTeamColorChooser(int rawValue)
    {
        var colorType = (ColorType)rawValue;

        return colorType switch
        {
            ColorType.JustWhite => JustWhite,
            ColorType.GetHairColor => GetHairColor,
            ColorType.GetSkinColor => GetSkinColor,
            ColorType.GetBodyColor => GetBodyColor,

            _ => Helpers.ThrowUnknownEnumValueException<ColorType, TeamColorChooser>(rawValue)
        };
    }

    public delegate Color TeamColorChooser(Team? team);

    public static readonly TeamColorChooser JustWhite = ReturnWhite;
    public static readonly TeamColorChooser GetHairColor = ReturnHairColor;
    public static readonly TeamColorChooser GetSkinColor = ReturnSkinColor;
    public static readonly TeamColorChooser GetBodyColor = ReturnBodyColor;

    private static Color ReturnWhite(Team? _) => Color.White;

    private static Color ReturnHairColor(Team? team)
    {
        return team?.HairColor ?? Color.White;
    }

    private static Color ReturnSkinColor(Team? team)
    {
        return team?.SkinColor ?? Color.White;
    }

    private static Color ReturnBodyColor(Team? team)
    {
        return team?.BodyColor ?? Color.White;
    }
}
