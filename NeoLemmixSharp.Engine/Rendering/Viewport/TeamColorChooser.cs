using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.Rendering.Viewport;

public abstract class TeamColorChooser
{
    private enum ColorChooserType
    {
        JustWhite,
        GetHairColor,
        GetSkinColor,
        GetBodyColor,
        GetFootColor,
        GetPaintColor
    }

    public static TeamColorChooser GetTeamColorChooser(int rawValue)
    {
        var colorType = (ColorChooserType)rawValue;

        return colorType switch
        {
            ColorChooserType.JustWhite => JustWhite,
            ColorChooserType.GetHairColor => GetHairColor,
            ColorChooserType.GetSkinColor => GetSkinColor,
            ColorChooserType.GetBodyColor => GetBodyColor,
            ColorChooserType.GetFootColor => GetFootColor,
            ColorChooserType.GetPaintColor => GetPaintColor,

            _ => Helpers.ThrowUnknownEnumValueException<ColorChooserType, TeamColorChooser>(rawValue)
        };
    }

    public static readonly TeamColorChooser JustWhite = new ReturnWhite();
    public static readonly TeamColorChooser GetHairColor = new ReturnHairColor();
    public static readonly TeamColorChooser GetSkinColor = new ReturnSkinColor();
    public static readonly TeamColorChooser GetBodyColor = new ReturnBodyColor();
    public static readonly TeamColorChooser GetFootColor = new ReturnFootColor();
    public static readonly TeamColorChooser GetPaintColor = new ReturnPaintColor();

    public abstract Color ChooseColor(Team? team);
    public abstract Color ChooseColor(Lemming lemming);

    private sealed class ReturnWhite : TeamColorChooser
    {
        public override Color ChooseColor(Team? _) => Color.White;
        public override Color ChooseColor(Lemming lemming) => Color.White;
    }

    private sealed class ReturnHairColor : TeamColorChooser
    {
        public override Color ChooseColor(Team? team) => team?.HairColor ?? Color.White;
        public override Color ChooseColor(Lemming lemming) => lemming.State.HairColor;
    }

    private sealed class ReturnSkinColor : TeamColorChooser
    {
        public override Color ChooseColor(Team? team) => team?.SkinColor ?? Color.White;
        public override Color ChooseColor(Lemming lemming) => lemming.State.SkinColor;
    }

    private sealed class ReturnBodyColor : TeamColorChooser
    {
        public override Color ChooseColor(Team? team) => team?.BodyColor ?? Color.White;
        public override Color ChooseColor(Lemming lemming) => lemming.State.BodyColor;
    }

    private sealed class ReturnFootColor : TeamColorChooser
    {
        public override Color ChooseColor(Team? _) => Color.White;
        public override Color ChooseColor(Lemming lemming) => lemming.State.FootColor;
    }

    private sealed class ReturnPaintColor : TeamColorChooser
    {
        public override Color ChooseColor(Team? team) => team?.PaintColor ?? Color.White;
        public override Color ChooseColor(Lemming lemming) => lemming.State.PaintColor;
    }
}
