using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Ui.Data;

public static class UiConstants
{
    public const string NumericTextFieldMask = "0123456789";
    public const string HexdecimalTextFieldMask = "0123456789ABCDEF";

    public const int StandardButtonHeight = 32;

    public const int StandardInset = 8;
    public const int TwiceStandardInset = StandardInset * 2;

    public const int RaisedRectangleBorder = 2;

    public enum TextRenderMode
    {
        UseFont,
        UseSprites
    }

    public static ColorPacket RectangularButtonDefaultColours => new(
        0xff444444.AsAbgrColor(),
        0xff666666.AsAbgrColor(),
        0xff888888.AsAbgrColor(),
        0xff006600.AsAbgrColor());

    public static ColorPacket LighterRectangularButtonColours => new(
        0xffa3a3a3.AsAbgrColor(),
        0xffd6d6d6.AsAbgrColor(),
        0xfff9f9f9.AsAbgrColor(),
        0xff00aa00.AsAbgrColor());

    public static ColorPacket AllBlackColours => new(Color.Black, Color.Black, Color.Black, Color.Black);

    public const int KeyboardInputFrameDelay = 30;
}
