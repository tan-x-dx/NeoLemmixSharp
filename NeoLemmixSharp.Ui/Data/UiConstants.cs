using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Ui.Data;

public static class UiConstants
{
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

    public static ColorPacket AllBlackColours => new(Color.Black, Color.Black, Color.Black, Color.Black);

    public const int KeyboardInputFrameDelay = 30;
}
