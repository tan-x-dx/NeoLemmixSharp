using Microsoft.Xna.Framework;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Ui.Data;

public static class UiConstants
{
    public const int StandardButtonHeight = 32;

    public const int StandardInset = 8;
    public const int TwiceStandardInset = StandardInset << 1;

    public const int RaisedRectangleBorder = 2;

    private const float FontGlyphWidth = 26f;
    internal const float FontScaleFactor = 0.3f;
    internal const float FontGlyphWidthMultiplier = FontGlyphWidth * FontScaleFactor;

    public static ColorPacket RectangularButtonDefaultColours => new(
       new Color((byte)0x44, (byte)0x44, (byte)0x44, (byte)0xff),
       new Color((byte)0x66, (byte)0x66, (byte)0x66, (byte)0xff),
       new Color((byte)0x88, (byte)0x88, (byte)0x88, (byte)0xff),
       new Color((byte)0x00, (byte)0x66, (byte)0x00, (byte)0xff));
}
