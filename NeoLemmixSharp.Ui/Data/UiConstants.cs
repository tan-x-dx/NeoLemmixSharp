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

    public const int DefaultTextXOffset = 4;
    public const int DefaultTextYOffset = 6;

    public const int RaisedRectangleBorder = 2;

    public static ColorPacket RectangularButtonDefaultColors => new(
        0xff444444,
        0xff666666,
        0xff888888,
        0xff006600);

    public static ColorPacket LighterRectangularButtonColors => new(
        0xffa3a3a3,
        0xffd6d6d6,
        0xfff9f9f9,
        0xff00aa00);

    public static ColorPacket AllWhiteColors => new(Color.White);
    public static ColorPacket AllBlackColors => new(Color.Black);

    public const int KeyboardInputFrameDelay = 30;
}
