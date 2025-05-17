using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Tribes;

namespace NeoLemmixSharp.Engine.Rendering.Viewport;

public abstract class TribeColorChooser
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

    public static TribeColorChooser GetTribeColorChooser(int rawValue)
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

            _ => Helpers.ThrowUnknownEnumValueException<ColorChooserType, TribeColorChooser>(colorType)
        };
    }

    public static readonly TribeColorChooser JustWhite = new ReturnWhite();
    public static readonly TribeColorChooser GetHairColor = new ReturnHairColor();
    public static readonly TribeColorChooser GetSkinColor = new ReturnSkinColor();
    public static readonly TribeColorChooser GetBodyColor = new ReturnBodyColor();
    public static readonly TribeColorChooser GetFootColor = new ReturnFootColor();
    public static readonly TribeColorChooser GetPaintColor = new ReturnPaintColor();

    public abstract Color ChooseColor(Tribe? tribe);
    public abstract Color ChooseColor(Lemming lemming);

    private sealed class ReturnWhite : TribeColorChooser
    {
        public override Color ChooseColor(Tribe? _) => Color.White;
        public override Color ChooseColor(Lemming lemming) => Color.White;
    }

    private sealed class ReturnHairColor : TribeColorChooser
    {
        public override Color ChooseColor(Tribe? tribe) => tribe?.HairColor ?? Color.White;
        public override Color ChooseColor(Lemming lemming) => lemming.State.HairColor;
    }

    private sealed class ReturnSkinColor : TribeColorChooser
    {
        public override Color ChooseColor(Tribe? tribe) => tribe?.SkinColor ?? Color.White;
        public override Color ChooseColor(Lemming lemming) => lemming.State.SkinColor;
    }

    private sealed class ReturnBodyColor : TribeColorChooser
    {
        public override Color ChooseColor(Tribe? tribe) => tribe?.BodyColor ?? Color.White;
        public override Color ChooseColor(Lemming lemming) => lemming.State.BodyColor;
    }

    private sealed class ReturnFootColor : TribeColorChooser
    {
        public override Color ChooseColor(Tribe? _) => Color.White;
        public override Color ChooseColor(Lemming lemming) => lemming.State.FootColor;
    }

    private sealed class ReturnPaintColor : TribeColorChooser
    {
        public override Color ChooseColor(Tribe? tribe) => tribe?.PaintColor ?? Color.White;
        public override Color ChooseColor(Lemming lemming) => lemming.State.PaintColor;
    }
}
